using System.Drawing;
using TimberLogicalBuilder.Core.Graph;
using TimberLogicalBuilder.Core.Model;
using TimberLogicalBuilder.Core.Structs;
using Timer = TimberLogicalBuilder.Core.Graph.Timer;

namespace TimberLogicalBuilder.Core.Builder;

public sealed class LogicLayout
  {
    public LogicBuilder Builder => _builder;
    public Vector3Int Cursor => _cursor;
    
    private readonly LogicBuilder _builder;
    private readonly Vector3Int _anchor;
    private Vector3Int _cursor;
    private readonly Vector3Int _primaryStep;
    private readonly Vector3Int _secondaryStep;
    private readonly Vector3Int _tertiaryStep;
    private readonly bool _autoAdvance;

    internal LogicLayout(
      LogicBuilder builder,
      Vector3Int anchor,
      Vector3Int primaryStep,
      Vector3Int secondaryStep,
      Vector3Int tertiaryStep,
      bool autoAdvance = true)
    {
      _builder = builder;
      _anchor = anchor;
      _cursor = anchor;
      _primaryStep = primaryStep;
      _secondaryStep = secondaryStep;
      _tertiaryStep =  tertiaryStep;
      _autoAdvance = autoAdvance;
    }

    #region Scope
    public Vector3Int Position => _cursor;

    internal void AdvancePrimary()
    {
      if (_autoAdvance)
      {
        _cursor += _primaryStep;
        _primaryIndex++;
      }
    }
    #endregion
  
    #region Cursor
    
    private int _primaryIndex;
    private int _secondaryIndex;
    private int _tertiaryIndex;
    
    public LogicLayout Step(int count = 1)
    {
      _cursor += _primaryStep * count;
      _primaryIndex += count;
      return this;
    }

    public LogicLayout NextRow(bool resetPrimary = true)
    {
      _cursor += _secondaryStep;
      _secondaryIndex++;
      if (resetPrimary && _primaryIndex != 0)
      {
        _cursor -= _primaryStep * _primaryIndex;
        _primaryIndex = 0;
      }
      return this;
    }
    
    public LogicLayout NextLayer(bool resetPrimary = true, bool resetSecondary = true)
    {
      _cursor += _tertiaryStep;
      _tertiaryIndex++;
      if (resetPrimary && _primaryIndex != 0)
      {
        _cursor -= _primaryStep * _primaryIndex;
        _primaryIndex = 0;
      }
      if (resetSecondary && _secondaryIndex != 0)
      {
        _cursor -= _secondaryStep * _secondaryIndex;
        _secondaryIndex = 0;
      }
      return this;
    }

    public LogicLayout Offset(Vector3Int offset)
    {
      _cursor += offset;
      return this;
    }

    public LogicLayout Reset()
    {
      _cursor = _anchor;
      _primaryIndex = 0;
      _secondaryIndex = 0;
      _tertiaryIndex = 0;
      return this;
    }
    #endregion
  
#region Placement
  #region Levers
  public Lever Lever(string name)
    => Build(() => _builder.Lever(name, Position));
  private Lever Build(Func<Lever> factory)
  {
    var lever = factory();
    AdvancePrimary();
    return lever;
  }
  #endregion
  
  #region Relays
  public Relay Passthrough(string name, ISignalSource input)
    => Build(() => _builder.Passthrough(name, Position, input));
  public Relay Not(string name, ISignalSource input)
    => Build(() => _builder.Not(name, Position, input));
  public Relay And(string name, ISignalSource inputA, ISignalSource inputB)
    => Build(() => _builder.And(name, Position, inputA, inputB));
  public Relay Or(string name, ISignalSource inputA, ISignalSource inputB)
    => Build(() => _builder.Or(name, Position, inputA, inputB));
  public Relay Xor(string name, ISignalSource inputA, ISignalSource inputB)
    => Build(() => _builder.Xor(name, Position, inputA, inputB));
  private Relay Build(Func<Relay> factory)
  {
    var relay = factory();
    AdvancePrimary();
    return relay;
  }
  #endregion
  
  #region Memory
  public Memory SetReset(string name, ISignalSource input, ISignalSource? reset = null)
    => Build(() => _builder.SetReset(name, Position, input, reset));
  public Memory Toggle(string name, ISignalSource input, ISignalSource? reset = null)
    => Build(() => _builder.Toggle(name, Position, input, reset));
  public Memory Latch(string name, ISignalSource inputA, ISignalSource inputB, ISignalSource? reset = null)
    => Build(() => _builder.Latch(name, Position, inputA, inputB, reset));
  public Memory FlipFlop(string name, ISignalSource inputA, ISignalSource inputB, ISignalSource? reset = null)
    => Build(() => _builder.FlipFlop(name, Position, inputA, inputB, reset));
  private Memory Build(Func<Memory> factory)
  {
    var memory = factory();
    AdvancePrimary();
    return memory;
  }
  #endregion

  #region Timer
  public Timer Pulse(string name, TimerInterval intervalA, ISignalSource input, ISignalSource? reset = null)
    => Build(() => _builder.Pulse(name, Position, intervalA, input, reset));
  public Timer Accumulator(string name, TimerInterval intervalA, ISignalSource input, ISignalSource? reset = null)
    => Build(() => _builder.Accumulator(name, Position, intervalA, input, reset));
  public Timer Delay(string name, TimerInterval intervalA, TimerInterval intervalB, ISignalSource input, ISignalSource? reset = null)
    => Build(() => _builder.Delay(name, Position, intervalA, intervalB, input, reset));
  public Timer Oscillator(string name, TimerInterval intervalA, TimerInterval intervalB, ISignalSource input, ISignalSource? reset = null)
    => Build(() =>  _builder.Oscillator(name, Position, intervalA, intervalB, input, reset));
  private Timer Build(Func<Timer> factory)
  {
    var timer = factory();
    AdvancePrimary();
    return timer;
  }
  #endregion

  #region Indicators
  public Indicator Indicator(string name, ISignalSource input, Color? color = null)
    => Build(() => _builder.Indicator(name, Position, input, color));
  private Indicator Build(Func<Indicator> factory)
  {
    var indicator = factory();
    AdvancePrimary();
    return indicator;
  }
  #endregion
#endregion
}