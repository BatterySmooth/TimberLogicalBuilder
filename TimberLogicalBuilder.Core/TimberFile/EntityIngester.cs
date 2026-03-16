using System.Data;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using TimberLogicalBuilder.Core.Graph;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.TimberFile;

public class EntityIngester
{
  // Todo: ingest the rest of the logic node types
  private static string logicNodeRegex = @"^(Relay|Memory|Lever|Indicator|Timer).*";

  // EntityIngester is stateful - it'll keep track of entities it's already seen, and entities it's looking for,
  // and try to wire everything up as it goes.
  // It's expecting to be called with every ingestible entity in a map in basically one go, rather than piecemeal
  private Dictionary<Guid, ISignalSource> sourcesById = [];
  private Dictionary<Guid, HashSet<(LogicNode, int)>> desiredInputs = [];

  public static bool isIngestible(JsonNode entity)
  {
    if(entity is null)
    {
      return false;
    }

    string entityType = TimberEntity.GetType(entity);
    Match match = Regex.Match(entityType, logicNodeRegex);
    return match.Success;
  }

  public LogicNode? Ingest(JsonNode entity)
  {
    string entityType = TimberEntity.GetType(entity);
    Match match = Regex.Match(entityType, logicNodeRegex);
    string factionlessType = match.Groups[1].Value;

    string name = TimberEntity.GetName(entity) ?? "";
    Vector3Int loc = TimberEntity.GetLoc(entity);
    Guid id = TimberEntity.GetId(entity);

    LogicNode? ent = null;
    string? inputAId = null;
    string? inputBId = null;
    string? resetInputId = null;

    switch(factionlessType)
    {
      case "Relay":
        ent = new LogicNode(NodeType.Relay, name, loc).SetRelayMode(TimberEntity.GetRelayMode(entity));
        ent.Id = id;
        sourcesById[id] = (ISignalSource) ent;
        inputAId = entity["Components"]?["Relay"]?["InputA"]?.AsValue().ToString();
        inputBId = entity["Components"]?["Relay"]?["InputB"]?.AsValue().ToString();
        break;
      case "Memory":
        ent = new LogicNode(NodeType.Memory, name, loc).SetMemoryMode(TimberEntity.GetMemoryMode(entity));
        ent.Id = id;
        sourcesById[id] = (ISignalSource) ent;
        inputAId = entity["Components"]?["Memory"]?["InputA"]?.AsValue().ToString();
        inputBId = entity["Components"]?["Memory"]?["InputB"]?.AsValue().ToString();
        resetInputId = entity["Components"]?["Memory"]?["ResetInput"]?.AsValue().ToString();
        break;
      case "Lever":
        Boolean.TryParse(entity["Components"]?["Lever"]?["IsSpringReturn"]?.AsValue().ToString(), out bool isReturn);
        Boolean.TryParse(entity["Components"]?["Lever"]?["isPinned"]?.AsValue().ToString(), out bool isPinned);
        ent = new LogicNode(NodeType.Lever, name, loc).SetIsSpringReturn(isReturn).SetIsPinned(isPinned);
        ent.Id = id;
        sourcesById[id] = (ISignalSource) ent;
        break;
      case "Indicator":
        ent = new LogicNode(NodeType.Indicator, name, loc);
        ent.Id = id;
        inputAId = entity["Components"]?["Automatable"]?["Input"]?.AsValue().ToString();
        break;
      case "Timer":
        ent = new LogicNode(name, loc).SetTimerMode(TimberEntity.GetTimerMode(entity))
          .SetTimerIntervalA(TimberEntity.GetTimerInterval(entity, true) ?? new TimerInterval(1, TimerUnit.Hours));
        if (TimberEntity.GetTimerInterval(entity, false).HasValue)
          ent.SetTimerIntervalB(TimberEntity.GetTimerInterval(entity, false)!.Value);
        ent.Id = id;
        sourcesById[id] = (ISignalSource) ent;
        break;
      case "HttpAdapter":
        ent = new LogicNode(NodeType.HttpAdapter, name, loc);
        ent.Id = id;
        inputAId = entity["Components"]?["Automatable"]?["Input"]?.AsValue().ToString();
        break;
      case "HttpLever":
        ent = new LogicNode(NodeType.HttpLever, name, loc);
        ent.Id = id;
        sourcesById[id] = (ISignalSource) ent;
        break;
    }

    // After all that, if we actually found the entity, time for wiring inputs and outputs
    if (ent is not null)
    {
      if (inputAId is not null)
      {
        Guid.TryParse(inputAId, out Guid inputAGuid);

        if (sourcesById.TryGetValue(inputAGuid, out ISignalSource? inputA))
        {
          ent.InputA = inputA;
        }
        else
        {
          if(!desiredInputs.TryGetValue(inputAGuid, out HashSet<(LogicNode, int)>? set))
          {
             set = [];
             desiredInputs[inputAGuid] = set;
          }

          set.Add((ent, 1));
        }
      }

      if (inputBId is not null)
      {
        Guid.TryParse(inputBId, out Guid inputBGuid);

        if (sourcesById.TryGetValue(inputBGuid, out ISignalSource? inputB))
        {
          ent.InputB = inputB;
        }
        else
        {
          if(!desiredInputs.TryGetValue(inputBGuid, out HashSet<(LogicNode, int)>? set))
          {
             set = [];
             desiredInputs[inputBGuid] = set;
          }

          set.Add((ent, 2));
        }
      }

      if (resetInputId is not null)
      {
        Guid.TryParse(resetInputId, out Guid resetGuid);

        if (sourcesById.TryGetValue(resetGuid, out ISignalSource? resetter))
        {
          ent.ResetInput = resetter;
        }
        else
        {
          if(!desiredInputs.TryGetValue(resetGuid, out HashSet<(LogicNode, int)>? set))
          {
             set = [];
             desiredInputs[resetGuid] = set;
          }

          set.Add((ent, 3));
        }
      }

      // And finally, satisfy anybody looking for this node
      if(desiredInputs.TryGetValue(id, out HashSet<(LogicNode, int)>? seekers))
      {
        foreach ((LogicNode node, int pos) seeker in seekers)
        {
          if(seeker.pos == 1)
          {
            seeker.node.InputA = (ISignalSource) ent;
          }

          if(seeker.pos == 2)
          {
            seeker.node.InputB = (ISignalSource) ent;
          }

          if(seeker.pos == 3)
          {
            seeker.node.ResetInput = (ISignalSource) ent;
          }
        }
      }
    }

    return ent;
  }
}