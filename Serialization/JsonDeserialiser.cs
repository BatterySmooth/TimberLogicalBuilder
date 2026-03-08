// Ignore this crap, it was a shabby attempt at 2 am and a few drinks deep - need to redo it
using TimberLogicalBuilder.Core.Graph;
using TimberLogicalBuilder.Core.Model;
using TimberLogicalBuilder.Core.Structs;
using Timer = TimberLogicalBuilder.Core.Graph.Timer;

namespace TimberLogicalBuilder.Core.Serialization;

using System.Text.Json;

public class JsonDeserialiser
{
    private readonly LogicGraph _graph = new();
    private readonly Dictionary<Guid, LogicNode> _nodeLookup = new();

    public LogicGraph BuildFromJson(string json)
    {
        var nodes = JsonSerializer.Deserialize<List<NodeDto>>(json)
                    ?? throw new InvalidOperationException("JSON was null");

        // First pass: create all nodes
        foreach (var dto in nodes)
        {
            var node = CreateNode(dto);
            _nodeLookup[dto.Id] = node;
            _graph.Add(node);
        }

        // Second pass: resolve connections
        foreach (var dto in nodes)
        {
            ResolveConnections(dto, _nodeLookup[dto.Id]);
        }

        return _graph;
    }

    private LogicNode CreateNode(NodeDto dto)
    {
        var pos = new Vector3Int(dto.Components.BlockObject.Coordinates.X,
                                 dto.Components.BlockObject.Coordinates.Y,
                                 dto.Components.BlockObject.Coordinates.Z);

        switch (dto.Template.Split('.')[0]) // Template like "Lever.Folktails"
        {
            case "Lever":
                bool spring = dto.Components.Relay?.Mode == "Spring"; // optional logic
                bool pinned = dto.Components.Relay?.Mode == "Pinned";
                return new Lever(dto.Components.NamedEntity.EntityName, pos, spring, pinned);

            case "Relay":
                RelayMode relayMode = dto.Components.Relay?.Mode switch
                {
                    "Not" => RelayMode.Not,
                    "And" => RelayMode.And,
                    "Or" => RelayMode.Or,
                    "Xor" => RelayMode.Xor,
                    "Passthrough" => RelayMode.Passthrough,
                    _ => throw new InvalidOperationException($"Unknown relay mode: {dto.Components.Relay?.Mode}")
                };
                return new Relay(dto.Components.NamedEntity.EntityName, pos, relayMode);

            case "Memory":
                MemoryMode memMode = dto.Components.Memory!.Mode switch
                {
                    "SetReset" => MemoryMode.SetReset,
                    "Toggle" => MemoryMode.Toggle,
                    "Latch" => MemoryMode.Latch,
                    "FlipFlop" => MemoryMode.FlipFlop,
                    _ => throw new InvalidOperationException($"Unknown memory mode: {dto.Components.Memory!.Mode}")
                };
                return new Memory(dto.Components.NamedEntity.EntityName, pos, memMode);

            case "Timer":
                return new Timer(dto.Components.NamedEntity.EntityName, pos, TimerMode.Oscillator,
                                 dto.Components.Timer!.TimerIntervalA.Ticks,
                                 dto.Components.Timer.TimerIntervalB.Ticks);

            case "Indicator":
                return new Indicator(dto.Components.NamedEntity.EntityName, pos);

            default:
                throw new InvalidOperationException($"Unknown template: {dto.Template}");
        }
    }

    private void ResolveConnections(NodeDto dto, LogicNode node)
    {
        switch (node)
        {
            case Relay relay when dto.Components.Relay != null:
            {
                if (dto.Components.Relay.InputA.HasValue)
                    relay.Inputs((ISignalSource)_nodeLookup[dto.Components.Relay.InputA.Value], 
                        dto.Components.Relay.InputB.HasValue ? (ISignalSource?)_nodeLookup[dto.Components.Relay.InputB.Value] : null);
                break;
            }
            case Memory mem when dto.Components.Memory != null:
            {
                if (dto.Components.Memory.InputA.HasValue)
                    mem.Inputs((ISignalSource)_nodeLookup[dto.Components.Memory.InputA.Value],
                        dto.Components.Memory.InputB.HasValue ? (ISignalSource)_nodeLookup[dto.Components.Memory.InputB.Value] : null);
                if (dto.Components.Memory.ResetInput.HasValue)
                    mem.Reset((ISignalSource)_nodeLookup[dto.Components.Memory.ResetInput.Value]);
                break;
            }
            case Timer timer when dto.Components.Timer != null:
            {
                if (dto.Components.Timer.Input.HasValue)
                    timer.InputSignal((ISignalSource)_nodeLookup[dto.Components.Timer.Input.Value]);
                if (dto.Components.Timer.ResetInput.HasValue)
                    timer.Reset((ISignalSource)_nodeLookup[dto.Components.Timer.ResetInput.Value]);
                break;
            }
            case Indicator ind when dto.Components.Automatable != null:
                ind.Connect((ISignalSource)_nodeLookup[dto.Components.Automatable.Input]);
                break;
        }
    }
}