using System.Data;
using System.Text.Json;
using System.Text.Json.Nodes;
using TimberLogicalBuilder.Core.Graph;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.TimberFile;

public class TimberEntity
{
  
  public static string getType(JsonNode entity)
  {
    return entity["Template"]?.AsValue().ToString() ?? "[Typeless]";
  }

  public static string? getName(JsonNode entity)
  {
    return entity["Components"]?["NamedEntity"]?["EntityName"]?.ToString();
  }

  public static string? getTemplate(JsonNode entity)
  {
    return entity["Template"]?.ToString();
  }

// For example: Beavers don't have a BlockObject, they have a Character.
  public static bool hasLoc(JsonNode entity)
  {
    JsonNode? location = entity["Components"]?["BlockObject"]?["Coordinates"];

    return (location != null);
  }
  public static Vector3Int getLoc(JsonNode entity)
  {
    JsonNode? location = entity["Components"]?["BlockObject"]?["Coordinates"];

    if (location == null)
    {
      throw new NoNullAllowedException();
    }

    int x = (int) (location["X"] ?? "-1");
    int y = (int) (location["Y"] ?? "-1");
    int z = (int) (location["Z"] ?? "-1");

    if(x == -1 || y == -1 || z == -1)
    {
      throw new NoNullAllowedException();
    }

    return new Vector3Int(x, y, z);
  }

  public static Guid getId(JsonNode entity)
  {
    // An entity without an ID? Unheard of!
    string id = entity["Id"]?.AsValue().ToString() ?? "[unidentified]";

    if(id != "[unidentified]")
    {
      return Guid.Parse(id);
    }
    
    return Guid.NewGuid();
  }

  public static RelayMode getRelayMode(JsonNode relay)
  {
    RelayMode.TryParse(relay["Components"]?["Relay"]?["Mode"]?.AsValue().ToString(), out RelayMode mode);
    return mode;
  }

  public static MemoryMode getMemoryMode(JsonNode memory)
  {
    MemoryMode.TryParse(memory["Components"]?["Memory"]?["Mode"]?.AsValue().ToString(), out MemoryMode mode);
    return mode;
  }

  public static TimerMode getTimerMode(JsonNode timer)
  {
    TimerMode.TryParse(timer["Components"]?["Timer"]?["Mode"]?.AsValue().ToString(), out TimerMode mode);
    return mode;
  }

  public static TimerInterval? getTimerInterval(JsonNode timer, bool first)
  {
    JsonNode? timerData = timer["Components"]?["Timer"];
    JsonNode? intervalJson;

    if (first)
    {
      intervalJson = timerData?["TimerIntervalA"];
    }
    else
    {
      intervalJson = timerData?["TimerIntervalB"];
    }

    if(intervalJson == null)
    {
      return null;
    }
    Enum.TryParse<TimerUnit>(intervalJson["Type"]?.AsValue().ToString(), out TimerUnit unit);
    int.TryParse(intervalJson[unit.ToString()]?.AsValue().ToString(), out int time);

    return new TimerInterval(time, unit);
  }
}