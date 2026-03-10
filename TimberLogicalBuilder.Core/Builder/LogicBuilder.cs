using System.Drawing;
using TimberLogicalBuilder.Core.Graph;
using TimberLogicalBuilder.Core.Model;
using TimberLogicalBuilder.Core.Structs;
using Timer = TimberLogicalBuilder.Core.Graph.Timer;

namespace TimberLogicalBuilder.Core.Builder;

public class LogicBuilder
{
  private readonly LogicGraph _graph = new();
  
  public LogicGraph Build() => _graph;
  
#region General
  #region Empty
  public Empty Empty(string name, Vector3Int position)
  {
    var empty = new Empty(name, position);
    return _graph.Add(empty);
  }
  #endregion

  #region Levers
  public Lever Lever(string name, Vector3Int position)
  {
    var lever = new Lever(name, position);
    return _graph.Add(lever);
  }
  #endregion

  #region Relays
  public Relay Passthrough(string name, Vector3Int position, ISignalSource input)
    => AddSingleRelay(RelayMode.Passthrough, name, position, input);
  public Relay Not(string name, Vector3Int position, ISignalSource input)
    => AddSingleRelay(RelayMode.Not, name, position, input);
  public Relay And(string name, Vector3Int position, ISignalSource inputA, ISignalSource inputB)
    => AddDualRelay(RelayMode.And, name, position, inputA, inputB);
  public Relay Or(string name, Vector3Int position, ISignalSource inputA, ISignalSource inputB)
    => AddDualRelay(RelayMode.Or, name, position, inputA, inputB);
  public Relay Xor(string name, Vector3Int position, ISignalSource inputA, ISignalSource inputB)
    => AddDualRelay(RelayMode.Xor, name, position, inputA, inputB);
  private Relay AddSingleRelay(RelayMode mode, string name, Vector3Int position, ISignalSource inputA)
  {
    var relay = new Relay(name, position, mode).Inputs(inputA);
    return _graph.Add(relay);
  }
  private Relay AddDualRelay(RelayMode mode, string name, Vector3Int position, ISignalSource inputA, ISignalSource inputB)
  {
    var relay = new Relay(name, position, mode).Inputs(inputA, inputB);
    return _graph.Add(relay);
  }
  #endregion
  
  #region Memory
  public Memory SetReset(string name, Vector3Int position, ISignalSource input, ISignalSource? reset = null)
    => AddSingleMemory(MemoryMode.SetReset, name, position, input, reset);
  public Memory Toggle(string name, Vector3Int position, ISignalSource input, ISignalSource? reset = null)
    => AddSingleMemory(MemoryMode.Toggle, name, position, input, reset);
  public Memory Latch(string name, Vector3Int position, ISignalSource inputA, ISignalSource inputB, ISignalSource? reset = null)
    => AddDualMemory(MemoryMode.Latch, name, position, inputA, inputB, reset);
  public Memory FlipFlop(string name, Vector3Int position, ISignalSource inputA, ISignalSource inputB, ISignalSource? reset = null)
    => AddDualMemory(MemoryMode.FlipFlop, name, position, inputA, inputB, reset);
  private Memory AddSingleMemory(MemoryMode mode, string name, Vector3Int position, ISignalSource input, ISignalSource? reset = null)
  {
    var memory = new Memory(name, position, mode).Inputs(input);
    if (reset is not null) memory.Reset(reset);
    return _graph.Add(memory);
  }
  private Memory AddDualMemory(MemoryMode mode, string name, Vector3Int position, ISignalSource inputA, ISignalSource inputB, ISignalSource? reset = null)
  {
    var memory = new Memory(name, position, mode).Inputs(inputA, inputB);
    if (reset is not null) memory.Reset(reset);
    return _graph.Add(memory);
  }
  #endregion

  #region Timer
  public Timer Pulse(string name, Vector3Int position, TimerInterval intervalA, ISignalSource input, ISignalSource? reset = null)
    => AddSingleTimer(TimerMode.Pulse, name, position, intervalA, input, reset);
  public Timer Accumulator(string name, Vector3Int position, TimerInterval intervalA, ISignalSource input, ISignalSource? reset = null)
    => AddSingleTimer(TimerMode.Accumulator, name, position, intervalA, input, reset);
  public Timer Delay(string name, Vector3Int position, TimerInterval intervalA, TimerInterval intervalB, ISignalSource input, ISignalSource? reset = null)
    => AddDualTimer(TimerMode.Delay, name, position, intervalA, intervalB, input, reset);
  public Timer Oscillator(string name, Vector3Int position, TimerInterval intervalA, TimerInterval intervalB, ISignalSource input, ISignalSource? reset = null)
    => AddDualTimer(TimerMode.Oscillator, name, position, intervalA, intervalB, input, reset);
  private Timer AddSingleTimer(TimerMode mode, string name, Vector3Int position, TimerInterval intervalA, ISignalSource input, ISignalSource? reset = null)
  {
    var delay = new Timer(name, position, mode, intervalA).InputSignal(input);
    if  (reset is not null) delay.Reset(reset);
    return _graph.Add(delay);
  }
  private Timer AddDualTimer(TimerMode mode, string name, Vector3Int position, TimerInterval intervalA, TimerInterval intervalB, ISignalSource input, ISignalSource? reset = null)
  {
    var delay = new Timer(name, position, mode, intervalA, intervalB).InputSignal(input);
    if  (reset is not null) delay.Reset(reset);
    return _graph.Add(delay);
  }
  #endregion

  #region Indicators
  public Indicator Indicator(string name, Vector3Int position, ISignalSource input, Color? color = null)
  {
    var indicator = new Indicator(name, position).Connect(input);
    if (color is not null) indicator.Color(color.Value);
    _graph.Add(indicator);
    return indicator;
  }
  #endregion
#endregion

#region Layout
  public LogicBuilder Layout(
    Vector3Int anchor,
    Vector3Int primaryStep,
    Vector3Int secondaryStep,
    Vector3Int tertiaryStep,
    Action<LogicLayout> layout)
  {
    var scope = new LogicLayout(
      this,
      anchor,
      primaryStep,
      secondaryStep,
      tertiaryStep);
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
    
    return Layout(anchor, primary, secondary, tertiary, layout);
    
    Vector3Int AxisVector(LayoutAxis axis) => axis switch
    {
      LayoutAxis.X => new Vector3Int(spacing, 0, 0),
      LayoutAxis.Y => new Vector3Int(0, spacing, 0),
      LayoutAxis.Z => new Vector3Int(0, 0, spacing),
      _ => throw new ArgumentOutOfRangeException()
    };
  }
#endregion

}