using System.Drawing;
using TimberLogicalBuilder.Core.Builder;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Demo;

class Program
{
  public const int BaseZ = 2; 
  static void Main(string[] args)
  {
    var builder = new LogicBuilder();
    
    BuildClock(builder);
    BuildRam(builder);

    var graph = builder.Build();
  }
  
  private static void BuildClock(LogicBuilder builder)
  {
    Vector3Int posAnchor = (4, 4, BaseZ);

    var clockManLev = builder
      .Lever("Clock (Manual)", posAnchor)
      .Sprung()
      .Pinned();
    var clockAutoLev = builder
      .Lever("Clock (Auto)", posAnchor + (2, 0, 0))
      .Pinned();
    var oscillator = builder
      .Oscillator("CLK-OSCILLATOR", posAnchor + (2, 1, 0), 
        (1, TimerUnit.Ticks), (1, TimerUnit.Ticks), clockAutoLev)
      .Covered();
    var notClockAuto =  builder
      .Not("!Clock (Auto)", posAnchor + (1, 1, 0), clockAutoLev)
      .Covered();
    var clockManOut = builder
      .And("CLK-MAN", posAnchor + (0, 1, 0), notClockAuto, clockManLev)
      .Covered();
    var clock = builder
      .Or("CLK", posAnchor + (1, 0, 0), oscillator, clockManOut)
      .Covered();
    var lamp = builder
      .Indicator("Clock", posAnchor + (1, 0, 2), clock, Color.DeepPink);
  }

  private static void BuildRam(LogicBuilder builder)
  {
    
  }
}