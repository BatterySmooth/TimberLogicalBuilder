using System.Drawing;
using TimberLogicalBuilder.Components.ComponentSystem;
using TimberLogicalBuilder.Core.Graph;

namespace TimberLogicalBuilder.Components.Components.Display;

// The stackup:
// [Display cube]
// [Memory unit]

public record zeroBitOutput();

public class Display1(
  string identifier, 
  ISignalSource input, 
  ISignalSource writeEnable, 
  Color? color = null)
   : BaseComponent<zeroBitOutput>
{
  public override zeroBitOutput Build(ComponentContext context)
  {
    var layout = context.RequireLayout();
    
    // FlipFlop(string name, Vector3Int position, ISignalSource inputA, ISignalSource inputB, ISignalSource? reset = null)
    var mem = context.Builder.Latch(
      identifier + "_mem",
      layout.Position,
      input,
      writeEnable
    ).Covered();


    // string name, Vector3Int position, ISignalSource input, Color? color = null
    var disp = context.Builder.Indicator(
      identifier+"_i",
      layout.Position + (0, 0, 2),
      mem,
      color
    );

    return new zeroBitOutput();
  }
}
