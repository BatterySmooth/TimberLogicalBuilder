using System.Drawing;
using TimberLogicalBuilder.Components.Components.Display;
using TimberLogicalBuilder.Components.Components.Memory;
using TimberLogicalBuilder.Components.Extensions;
using TimberLogicalBuilder.Components.Structs;
using TimberLogicalBuilder.Core.Builder;
using TimberLogicalBuilder.Core.TimberFile;
using TimberLogicalBuilder.Core.Model;
using TimberLogicalBuilder.Core.Serialization;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Demo;

class Program
{
  private const string InputSave = @"C:\Path\To\Save\File\Blank.timber";
  private const string OutputSave = @"C:\Path\To\Save\File\GeneratedSave.timber";
  private const int BaseZ = 4;
  
  static void Main(string[] args)
  {
    String infile = args[0] ?? InputSave;
    String outfile = args[1] ?? OutputSave;

    Console.WriteLine("Loading existing entities...");
    Dictionary<string, LogicNode> nodesByName = TimberSaveFile.LoadLogicNodes(infile);
    Console.WriteLine($"Found {nodesByName.Count} existing nodes.");

    var builder = new LogicBuilder(new LogicBuilderSettings().PreserveExistingConnections(), nodesByName);
    var clock = BuildClock(builder, (5, 5, BaseZ));

    LogicGraphSerializer.settings.faction = Faction.Folktails;

    builder.Layout((20, 20, BaseZ), LayoutAxis.X, LayoutAxis.Y, 1, l =>
    {
      l.Step();
      var inputs = BuildMemorySwitches(l);
      l.NextRow();
      l.NextRow();
      
      var memCount = 6;
      var channelCount = 2;
      int maxWidth = 16;
      var memSelects = new ISignalSource[memCount][];
      int placedInRow = 0;
      l.Step();
      for (var m = 0; m < memCount; m++)
      {
        memSelects[m] = new ISignalSource[channelCount];
        for (var c = 0; c < channelCount; c++)
        {
          if (placedInRow >= maxWidth)
          {
            l.NextRow();
            l.Step();
            placedInRow = 0;
          }
          memSelects[m][c] = l.Lever($"MEM {m} CHAN SEL {c}").Pinned();
          placedInRow++;
        }
      }

      l.NextRow();
      l.NextRow();

      Register16Output? previous = null;

      for (var m = 0; m < memCount; m++)
      {
        var memEnable = l.Lever($"MemEnable{m}").Sprung().Pinned();
        var output = l.Component(new Register16($"{m:D2}", memEnable, memSelects[m], previous, inputs));
        previous = output;
      }

      l.NextRow();
      
      BuildLamps(l, previous);
    });
    

    builder.Layout((20, 40, BaseZ), LayoutAxis.X, LayoutAxis.Y, 1, l =>
    {
      l.Step();
      var charInputs = BuildMemorySwitches(l);
      l.NextRow();
      l.NextRow();

      var fifteen = l.Component(new Hex215("hex", charInputs)).Channels;
      l.NextRow();
      ISignalSource dispPwr = l.Lever("display_on");
      l.NextRow();
      l.Component(new Display15("disp", fifteen, dispPwr));
    });


    Console.WriteLine("Building entity graph...");

    var graph = builder.Build();
    var output = LogicGraphSerializer.Serialize(graph);

    Console.WriteLine($"Writing to file: {outfile}");

    TimberSaveFile.IncludeEntities(infile, outfile, output);
  }

  private static void BuildLamps(LogicLayout layout, Register16Output bus)
  {
    foreach (var t in bus.Channels)
    {
      layout.Step();
      layout.Indicator("BUS-00", t.B0,  Color.DeepPink);
      layout.Indicator("BUS-01", t.B1,  Color.DeepPink);
      layout.Indicator("BUS-02", t.B2,  Color.DeepPink);
      layout.Indicator("BUS-03", t.B3,  Color.DeepPink);
      layout.Indicator("BUS-04", t.B4,  Color.DeepPink);
      layout.Indicator("BUS-05", t.B5,  Color.DeepPink);
      layout.Indicator("BUS-06", t.B6,  Color.DeepPink);
      layout.Indicator("BUS-07", t.B7,  Color.DeepPink);
      layout.Indicator("BUS-08", t.B8,  Color.DeepPink);
      layout.Indicator("BUS-09", t.B9,  Color.DeepPink);
      layout.Indicator("BUS-10", t.B10, Color.DeepPink);
      layout.Indicator("BUS-11", t.B11, Color.DeepPink);
      layout.Indicator("BUS-12", t.B12, Color.DeepPink);
      layout.Indicator("BUS-13", t.B13, Color.DeepPink);
      layout.Indicator("BUS-14", t.B14, Color.DeepPink);
      layout.Indicator("BUS-15", t.B15, Color.DeepPink);
      layout.NextRow();
      layout.NextRow();
    }
  }

  private static Word16 BuildMemorySwitches(LogicLayout layout)
  {
    var outputs = new ISignalSource[16];
    for (var x = 0; x < 16; x++)
      outputs[x] = layout.Lever($"MEM-IN-{x}").Pinned();
    return Word16.FromArray(outputs);
  }

  private static ISignalSource BuildClock(LogicBuilder builder, Vector3Int anchor)
  {
    var clockManLev = builder
      .Lever("Clock (Manual)", anchor)
      .Sprung()
      .Pinned();
    var clockAutoLev = builder
      .Lever("Clock (Auto)", anchor + (2, 0, 0))
      .Pinned();
    var oscillator = builder
      .Oscillator("CLK-OSCILLATOR", anchor + (2, 1, 0), (1, TimerUnit.Ticks), (1, TimerUnit.Ticks), clockAutoLev)
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
    builder
      .Indicator("Clock", anchor + (1, 0, 2), clock, Color.DeepPink)
      .Pinned();
    return clock;
  }
}