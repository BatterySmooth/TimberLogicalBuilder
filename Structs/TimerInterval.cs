namespace TimberLogicalBuilder.Core.Structs;

public readonly struct TimerInterval(int interval, TimerUnit unit)
{
  public int Interval { get; } = interval;
  public TimerUnit Unit { get; } = unit;
  
  public static implicit operator TimerInterval((int interval, TimerUnit unit) t)
    => new (t.interval, t.unit);
}