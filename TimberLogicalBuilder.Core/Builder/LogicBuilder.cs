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
    var empty = new LogicNode(name, position)
      .SetIsEmpty(true)
      .Covered();
    return _graph.Add(empty);
  }
  #endregion

  #region Levers
  public LogicNode Lever(string name, Vector3Int position)
  {
    var lever = Reuse(new LogicNode(name, position));
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
    var relay = Reuse(new LogicNode(name, position)
      .SetRelayMode(mode));
    if (inputA is not null) relay.ConnectA(inputA);
    if (inputB is not null) relay.ConnectB(inputB);
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
    var memory = Reuse(new LogicNode(name, position)
      .SetMemoryMode(mode));
    if (inputA is not null) memory.ConnectA(inputA);
    if (inputB is not null) memory.ConnectB(inputB);
    if (reset is not null) memory.ConnectReset(reset);
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
    var delay = Reuse(new LogicNode(name, position)
      .SetTimerMode(mode)
      .SetTimerIntervalA(intervalA));
    if (intervalB.HasValue) delay.SetTimerIntervalB(intervalB.Value);
    if (input is not null) delay.ConnectA(input);
    if (reset is not null) delay.ConnectReset(reset);
    return _graph.Add(delay);
  }
  #endregion

  #region Indicators
  public LogicNode Indicator(string name, Vector3Int position, ISignalSource? input = null, Color? color = null)
  {
    var indicator = Reuse(new LogicNode(name, position));
    if (input is not null) indicator.ConnectA(input);
    if (color is not null) indicator.Color(color.Value);
    _graph.Add(indicator);
    return indicator;
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