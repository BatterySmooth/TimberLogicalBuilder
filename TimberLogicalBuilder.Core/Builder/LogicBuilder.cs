using System.Drawing;
using TimberLogicalBuilder.Core.Graph;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.Builder;

public class LogicBuilder
{
  private readonly LogicGraph _graph = new();
  private readonly LogicBuilderSettings _settings;

  private readonly Dictionary<String, LogicNode> _nodesByName = [];
  
  public LogicGraph Build() => _graph;

#region Constructors
  public LogicBuilder(LogicBuilderSettings settings)
  {
    if (_settings.preserveExistingConnections)
    {
      throw new Exception("Cannot preserve existing connections if you don't give me any! Use the other constructor.");
    }

    _settings = settings;
  }

  public LogicBuilder(LogicBuilderSettings settings, Dictionary<String, LogicNode> nodesByName)
  {
    _nodesByName = nodesByName;
    _settings = settings;
  }
#endregion

#region Existing
  public LogicNode Reuse(LogicNode newbie)
  {
    if (!_settings.reuseExisting)
    {
      return newbie;
    }

    _nodesByName.TryGetValue(newbie.Name, out LogicNode? existing);

    if (existing == null)
    {
      return newbie;
    }

    if (existing.GetType() != newbie.GetType()) 
    {
      // Probably a name collision, stick with what's new
      return newbie;
    }

    ISignalSource? existingInputA = existing.InputA;
    ISignalSource? newInputA = newbie.InputA;

    if(!_settings.preserveExistingConnections || ( existingInputA == null && !_settings.preserveExistingNullConnections))
    {
      existing.InputA = newInputA;
    }

    ISignalSource? existingInputB = existing.InputB;
    ISignalSource? newInputB = newbie.InputB;

    if(!_settings.preserveExistingConnections || ( existingInputB == null && !_settings.preserveExistingNullConnections))
    {
      existing.InputB = newInputB;
    }

    ISignalSource? existingReset = existing.ResetInput;
    ISignalSource? newReset = newbie.ResetInput;

    if(!_settings.preserveExistingConnections || ( existingReset == null && !_settings.preserveExistingNullConnections))
    {
      existing.ResetInput = newReset;
    }

    return existing;
  }

#endregion

#region General
  #region Empty
  public LogicNode Empty(string name, Vector3Int position)
  {
    var empty = new LogicNode(NodeType.Empty, name, position)
      .Covered();
    return _graph.Add(empty);
  }
  #endregion

  #region Levers
  public LogicNode Lever(string name, Vector3Int position)
  {
    var lever = Reuse(new LogicNode(NodeType.Lever, name, position));
    return _graph.Add(lever);
  }
  #endregion

  #region Relays
  public LogicNode Passthrough(string name, Vector3Int position, ISignalSource? input = null)
    => AddRelay(RelayMode.Passthrough, name, position, inputA: input);
  public LogicNode Not(string name, Vector3Int position, ISignalSource? input = null)
    => AddRelay(RelayMode.Not, name, position, inputA: input);
  public LogicNode And(string name, Vector3Int position, ISignalSource? inputA = null, ISignalSource? inputB = null)
    => AddRelay(RelayMode.And, name, position, inputA: inputA, inputB: inputB);
  public LogicNode Or(string name, Vector3Int position, ISignalSource? inputA = null, ISignalSource? inputB = null)
    => AddRelay(RelayMode.Or, name, position, inputA: inputA, inputB: inputB);
  public LogicNode Xor(string name, Vector3Int position, ISignalSource? inputA = null, ISignalSource? inputB = null)
    => AddRelay(RelayMode.Xor, name, position, inputA: inputA, inputB: inputB);
  private LogicNode AddRelay(RelayMode mode, string name, Vector3Int position, ISignalSource? inputA = null, ISignalSource? inputB = null)
  {
    var relay = Reuse(new LogicNode(NodeType.Relay, name, position)
      .SetRelayMode(mode));
    if (inputA != null) relay.ConnectA(inputA);
    if (inputB != null) relay.ConnectB(inputB);
    return _graph.Add(relay);
  }
  #endregion
  
  #region Memory
  public LogicNode SetReset(string name, Vector3Int position, ISignalSource? input = null, ISignalSource? reset = null)
    => AddMemory(MemoryMode.SetReset, name, position, inputA: input, reset: reset);
  public LogicNode Toggle(string name, Vector3Int position, ISignalSource? input = null, ISignalSource? reset = null)
    => AddMemory(MemoryMode.Toggle, name, position, inputA: input, reset: reset);
  public LogicNode Latch(string name, Vector3Int position, ISignalSource? inputA = null, ISignalSource? inputB = null, ISignalSource? reset = null)
    => AddMemory(MemoryMode.Latch, name, position, inputA: inputA, inputB: inputB, reset: reset);
  public LogicNode FlipFlop(string name, Vector3Int position, ISignalSource? inputA = null, ISignalSource? inputB = null, ISignalSource? reset = null)
    => AddMemory(MemoryMode.FlipFlop, name, position, inputA: inputA, inputB: inputB, reset: reset);
  private LogicNode AddMemory(MemoryMode mode, string name, Vector3Int position, ISignalSource? inputA = null, ISignalSource? inputB = null, ISignalSource? reset = null)
  {
    var memory = Reuse(new LogicNode(NodeType.Memory, name, position)
      .SetMemoryMode(mode));
    if (inputA != null) memory.ConnectA(inputA);
    if (inputB != null) memory.ConnectB(inputB);
    if (reset != null) memory.ConnectReset(reset);
    return _graph.Add(memory);
  }
  #endregion

  #region Timer
  public LogicNode Pulse(string name, Vector3Int position, TimerInterval intervalA, ISignalSource? input = null, ISignalSource? reset = null)
    => AddTimer(TimerMode.Pulse, name, position, intervalA, input: input, reset: reset);
  public LogicNode Accumulator(string name, Vector3Int position, TimerInterval intervalA, ISignalSource? input = null, ISignalSource? reset = null)
    => AddTimer(TimerMode.Accumulator, name, position, intervalA, input: input, reset: reset);
  public LogicNode Delay(string name, Vector3Int position, TimerInterval intervalA, TimerInterval intervalB, ISignalSource? input = null, ISignalSource? reset = null)
    => AddTimer(TimerMode.Delay, name, position, intervalA, intervalB, input: input, reset: reset);
  public LogicNode Oscillator(string name, Vector3Int position, TimerInterval intervalA, TimerInterval intervalB, ISignalSource? input = null, ISignalSource? reset = null)
    => AddTimer(TimerMode.Oscillator, name, position, intervalA, intervalB, input: input, reset: reset);
  private LogicNode AddTimer(TimerMode mode, string name, Vector3Int position, TimerInterval intervalA, TimerInterval? intervalB = null, ISignalSource? input = null, ISignalSource? reset = null)
  {
    var delay = Reuse(new LogicNode(NodeType.Timer, name, position)
      .SetTimerMode(mode)
      .SetTimerIntervalA(intervalA));
    if (intervalB.HasValue) delay.SetTimerIntervalB(intervalB.Value);
    if (input != null) delay.ConnectA(input);
    if (reset != null) delay.ConnectReset(reset);
    return _graph.Add(delay);
  }
  #endregion

  #region Indicators
  public LogicNode Indicator(string name, Vector3Int position, ISignalSource? input = null, Color? color = null)
  {
    var indicator = Reuse(new LogicNode(NodeType.Indicator, name, position));
    if (input is not null) indicator.ConnectA(input);
    if (color is not null) indicator.Color(color.Value);
    _graph.Add(indicator);
    return indicator;
  }
  #endregion

  #region Http
  public LogicNode HttpLever(string name, Vector3Int position, Color? color = null)
  {
    var lever = Reuse(new LogicNode(NodeType.HttpLever, name, position));
    if(color is not null) lever.Color(color.Value);
    _graph.Add(lever);
    return lever;
  }

  public LogicNode HttpAdapter(string name, Vector3Int position, ISignalSource? input = null, string? UrlWhenOn = null, string? UrlWhenOff = null)
  {
    var adapter = Reuse(new LogicNode(NodeType.HttpAdapter, name, position));
    if(input is not null) adapter.ConnectA(input);
    if(UrlWhenOn is not null) adapter.SetWhenOnHttp(UrlWhenOn);
    if(UrlWhenOff is not null) adapter.SetWhenOffHttp(UrlWhenOff);
    _graph.Add(adapter);
    return adapter;
  }

  #endregion
#endregion

#region Layout
  public LogicBuilder Layout(Vector3Int anchor, LayoutAxes axes, Action<LogicLayout> layout)
  {
    var scope = new LogicLayout(this, anchor, axes);
    layout(scope);
    return this;
  }
  
  public LogicBuilder Layout(
    Vector3Int anchor,
    LayoutAxis primaryAxis,
    LayoutAxis secondaryAxis,
    int spacing,
    Action<LogicLayout> layout)
  {
    if (primaryAxis == secondaryAxis)
      throw new ArgumentException("Primary and secondary axes must be different.");
    
    var tertiaryAxis = Enum.GetValues<LayoutAxis>()
      .First(a => a != primaryAxis && a != secondaryAxis);

    var primary = AxisVector(primaryAxis);
    var secondary = AxisVector(secondaryAxis);
    var tertiary = AxisVector(tertiaryAxis);
    
    return Layout(anchor, (primary, secondary, tertiary), layout);
    
    Vector3Int AxisVector(LayoutAxis axis) => axis switch
    {
      LayoutAxis.X => new Vector3Int(spacing, 0, 0),
      LayoutAxis.Y => new Vector3Int(0, spacing, 0),
      LayoutAxis.Z => new Vector3Int(0, 0, Math.Max(spacing, 2)),
      _ => throw new ArgumentOutOfRangeException()
    };
  }
#endregion

}