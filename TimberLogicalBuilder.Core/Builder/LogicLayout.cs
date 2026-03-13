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
    public readonly LayoutAxes Axes;
    private readonly bool _autoAdvance;

    internal LogicLayout(
      LogicBuilder builder,
      Vector3Int anchor,
      LayoutAxes axes,
      bool autoAdvance = true)
    {
      _builder = builder;
      _anchor = anchor;
      _cursor = anchor;
      Axes = axes;
      _autoAdvance = autoAdvance;
    }

    #region Scope
    internal void AdvancePrimary()
    {
      if (_autoAdvance)
      {
        _cursor += Axes.Primary;
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
      _cursor += Axes.Primary * count;
      _primaryIndex += count;
      return this;
    }

    public LogicLayout NextRow(bool resetPrimary = true)
    {
      _cursor += Axes.Secondary;
      _secondaryIndex++;
      if (resetPrimary && _primaryIndex != 0)
      {
        _cursor -= Axes.Primary * _primaryIndex;
        _primaryIndex = 0;
      }
      return this;
    }
    
    public LogicLayout NextLayer(bool resetPrimary = true, bool resetSecondary = true)
    {
      _cursor += Axes.Tertiary;
      _tertiaryIndex++;
      if (resetPrimary && _primaryIndex != 0)
      {
        _cursor -= Axes.Primary * _primaryIndex;
        _primaryIndex = 0;
      }
      if (resetSecondary && _secondaryIndex != 0)
      {
        _cursor -= Axes.Secondary * _secondaryIndex;
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
    => Build(() => _builder.Empty(name, Cursor));
  #endregion

  #region Levers
  public LogicNode Lever(string name)
    => Build(() => _builder.Lever(name, Cursor));
  #endregion
  
  #region Relays
  public LogicNode Passthrough(string name, ISignalSource? input = null)
    => Build(() => _builder.Passthrough(name, Cursor, input));
  public LogicNode Not(string name, ISignalSource? input = null)
    => Build(() => _builder.Not(name, Cursor, input));
  public LogicNode And(string name, ISignalSource? inputA = null, ISignalSource? inputB = null)
    => Build(() => _builder.And(name, Cursor, inputA, inputB));
  public LogicNode Or(string name, ISignalSource? inputA = null, ISignalSource? inputB = null)
    => Build(() => _builder.Or(name, Cursor, inputA, inputB));
  public LogicNode Xor(string name, ISignalSource? inputA = null, ISignalSource? inputB = null)
    => Build(() => _builder.Xor(name, Cursor, inputA, inputB));
  #endregion
  
  #region Memory
  public LogicNode SetReset(string name, ISignalSource input, ISignalSource? reset = null)
    => Build(() => _builder.SetReset(name, Cursor, input, reset));
  public LogicNode Toggle(string name, ISignalSource input, ISignalSource? reset = null)
    => Build(() => _builder.Toggle(name, Cursor, input, reset));
  public LogicNode Latch(string name, ISignalSource? inputA = null, ISignalSource? inputB = null, ISignalSource? reset = null)
    => Build(() => _builder.Latch(name, Cursor, inputA, inputB, reset));
  public LogicNode FlipFlop(string name, ISignalSource? inputA = null, ISignalSource? inputB = null, ISignalSource? reset = null)
    => Build(() => _builder.FlipFlop(name, Cursor, inputA, inputB, reset));
  #endregion

  #region Timer
  public LogicNode Pulse(string name, TimerInterval intervalA, ISignalSource? input = null, ISignalSource? reset = null)
    => Build(() => _builder.Pulse(name, Cursor, intervalA, input, reset));
  public LogicNode Accumulator(string name, TimerInterval intervalA, ISignalSource? input = null, ISignalSource? reset = null)
    => Build(() => _builder.Accumulator(name, Cursor, intervalA, input, reset));
  public LogicNode Delay(string name, TimerInterval intervalA, TimerInterval intervalB, ISignalSource? input = null, ISignalSource? reset = null)
    => Build(() => _builder.Delay(name, Cursor, intervalA, intervalB, input, reset));
  public LogicNode Oscillator(string name, TimerInterval intervalA, TimerInterval intervalB, ISignalSource? input = null, ISignalSource? reset = null)
    => Build(() =>  _builder.Oscillator(name, Cursor, intervalA, intervalB, input, reset));
  #endregion

  #region Indicators
  public LogicNode Indicator(string name, ISignalSource? input = null, Color? color = null)
    => Build(() => _builder.Indicator(name, Cursor, input, color));
  #endregion
  
  private LogicNode Build(Func<LogicNode> factory)
  {
    var node = factory();
    AdvancePrimary();
    return node;
  }
#endregion
}