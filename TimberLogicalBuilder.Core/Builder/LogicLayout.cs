using System.Drawing;
using TimberLogicalBuilder.Core.Graph;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.Builder;

public sealed class LogicLayout
  {
    public LogicBuilder Builder => _builder;
    public Vector3Int Cursor => _cursor;
    
    private readonly LogicBuilder _builder;
    private readonly Vector3Int _anchor;
    private Vector3Int _cursor;
    public readonly Vector3Int PrimaryStep;
    public readonly Vector3Int SecondaryStep;
    public readonly Vector3Int TertiaryStep;
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
      PrimaryStep = primaryStep;
      SecondaryStep = secondaryStep;
      TertiaryStep =  tertiaryStep;
      _autoAdvance = autoAdvance;
    }

    #region Scope
    public Vector3Int Position => _cursor;

    internal void AdvancePrimary()
    {
      if (_autoAdvance)
      {
        _cursor += PrimaryStep;
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
      _cursor += PrimaryStep * count;
      _primaryIndex += count;
      return this;
    }

    public LogicLayout NextRow(bool resetPrimary = true)
    {
      _cursor += SecondaryStep;
      _secondaryIndex++;
      if (resetPrimary && _primaryIndex != 0)
      {
        _cursor -= PrimaryStep * _primaryIndex;
        _primaryIndex = 0;
      }
      return this;
    }
    
    public LogicLayout NextLayer(bool resetPrimary = true, bool resetSecondary = true)
    {
      _cursor += TertiaryStep;
      _tertiaryIndex++;
      if (resetPrimary && _primaryIndex != 0)
      {
        _cursor -= PrimaryStep * _primaryIndex;
        _primaryIndex = 0;
      }
      if (resetSecondary && _secondaryIndex != 0)
      {
        _cursor -= SecondaryStep * _secondaryIndex;
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
  #region Empty
  public LogicNode Empty(string name)
    => Build(() => _builder.Empty(name, Position));
  #endregion

  #region Levers
  public LogicNode Lever(string name)
    => Build(() => _builder.Lever(name, Position));
  #endregion
  
  #region Relays
  public LogicNode Passthrough(string name, ISignalSource? input = null)
    => Build(() => _builder.Passthrough(name, Position, input));
  public LogicNode Not(string name, ISignalSource? input = null)
    => Build(() => _builder.Not(name, Position, input));
  public LogicNode And(string name, ISignalSource? inputA = null, ISignalSource? inputB = null)
    => Build(() => _builder.And(name, Position, inputA, inputB));
  public LogicNode Or(string name, ISignalSource? inputA = null, ISignalSource? inputB = null)
    => Build(() => _builder.Or(name, Position, inputA, inputB));
  public LogicNode Xor(string name, ISignalSource? inputA = null, ISignalSource? inputB = null)
    => Build(() => _builder.Xor(name, Position, inputA, inputB));
  #endregion
  
  #region Memory
  public LogicNode SetReset(string name, ISignalSource input, ISignalSource? reset = null)
    => Build(() => _builder.SetReset(name, Position, input, reset));
  public LogicNode Toggle(string name, ISignalSource input, ISignalSource? reset = null)
    => Build(() => _builder.Toggle(name, Position, input, reset));
  public LogicNode Latch(string name, ISignalSource? inputA = null, ISignalSource? inputB = null, ISignalSource? reset = null)
    => Build(() => _builder.Latch(name, Position, inputA, inputB, reset));
  public LogicNode FlipFlop(string name, ISignalSource? inputA = null, ISignalSource? inputB = null, ISignalSource? reset = null)
    => Build(() => _builder.FlipFlop(name, Position, inputA, inputB, reset));
  #endregion

  #region Timer
  public LogicNode Pulse(string name, TimerInterval intervalA, ISignalSource? input = null, ISignalSource? reset = null)
    => Build(() => _builder.Pulse(name, Position, intervalA, input, reset));
  public LogicNode Accumulator(string name, TimerInterval intervalA, ISignalSource? input = null, ISignalSource? reset = null)
    => Build(() => _builder.Accumulator(name, Position, intervalA, input, reset));
  public LogicNode Delay(string name, TimerInterval intervalA, TimerInterval intervalB, ISignalSource? input = null, ISignalSource? reset = null)
    => Build(() => _builder.Delay(name, Position, intervalA, intervalB, input, reset));
  public LogicNode Oscillator(string name, TimerInterval intervalA, TimerInterval intervalB, ISignalSource? input = null, ISignalSource? reset = null)
    => Build(() =>  _builder.Oscillator(name, Position, intervalA, intervalB, input, reset));
  #endregion

  #region Indicators
  public LogicNode Indicator(string name, ISignalSource? input = null, Color? color = null)
    => Build(() => _builder.Indicator(name, Position, input, color));
  #endregion
  
  private LogicNode Build(Func<LogicNode> factory)
  {
    var node = factory();
    AdvancePrimary();
    return node;
  }
#endregion
}