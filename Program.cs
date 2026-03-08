using System.Drawing;
using TimberLogicalBuilder.Core.Builder;
using TimberLogicalBuilder.Core.Exporting;
using TimberLogicalBuilder.Core.Serialization;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Demo;

class Program
{
  private const string InputSave = @"C:\Path\To\Save\Folder\Blank.timber";
  private const string OutputSave = @"C:\Path\To\Save\Folder\GeneratedSave.timber";
  private const int BaseZ = 2;
  
  static void Main(string[] args)
  {
    var builder = new LogicBuilder();
    BuildClock(builder, (5, 5, BaseZ));
    var source = builder.Lever("Source", (1, 1, BaseZ));
    var grid = builder.Layout((5, 5, BaseZ), LayoutAxis.X, 1, l =>
    {
      for (var y = 0; y < 8; y++)
      {
        for (var x = 0; x < 8; x++)
        {
          l.Indicator($"LED_{x}_{y}", source);
        }
        l.NextRow();
      }
    });

    var graph = builder.Build();
    var output = LogicGraphSerializer.Serialize(graph);
    TimberSaveWriter.WriteEntities(InputSave, OutputSave, output);
  }
  
  private static void BuildClock(LogicBuilder builder, Vector3Int anchor)
  {
    var clockManLev = builder
      .Lever("Clock (Manual)", anchor)
      .Sprung()
      .Pinned();
    var clockAutoLev = builder
      .Lever("Clock (Auto)", anchor + (2, 0, 0))
      .Pinned();
    var oscillator = builder
      .Oscillator("CLK-OSCILLATOR", anchor + (2, 1, 0), 
        (1, TimerUnit.Ticks), (1, TimerUnit.Ticks), clockAutoLev)
      .Covered();
    var notClockAuto =  builder
      .Not("!Clock (Auto)", anchor + (1, 1, 0), clockAutoLev)
      .Covered();
    var clockManOut = builder
      .And("CLK-MAN", anchor + (0, 1, 0), notClockAuto, clockManLev)
      .Covered();
    var clock = builder
      .Or("CLK", anchor + (1, 0, 0), oscillator, clockManOut)
      .Covered();
    var lamp = builder
      .Indicator("Clock", anchor + (1, 0, 2), clock, Color.DeepPink)
      .Pinned();
  }
}