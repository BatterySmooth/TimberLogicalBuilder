using TimberLogicalBuilder.Core.Model;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.Graph;

public class Timer(string name, Vector3Int pos, TimerMode mode, TimerInterval intervalA, TimerInterval? intervalB = null) : LogicNode<Timer>(name, pos), ISignalSource
{
  public TimerMode Mode { get; } = mode;
  public TimerInterval IntervalA { get; } = intervalA;
  public TimerInterval? IntervalB { get; } = intervalB;

  public Timer InputSignal(ISignalSource input)
  {
    InputA = input;
    return this;
  }

  public Timer Reset(ISignalSource input)
  {
    ResetInput = input;
    return this;
  }

  public Timer Inputs(ISignalSource? input, ISignalSource? reset)
  {
    InputA = input;
    ResetInput = reset;
    return this;
  }
}