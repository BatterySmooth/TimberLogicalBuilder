namespace TimberLogicalBuilder.Core.Structs;

public struct TimerInterval(int interval, TimerUnit unit)
{
  public static implicit operator TimerInterval((int interval, TimerUnit unit) t) => new TimerInterval(t.interval, t.unit);
}