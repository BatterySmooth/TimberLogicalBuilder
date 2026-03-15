using System.Drawing;
using TimberLogicalBuilder.Components.ComponentSystem;
using TimberLogicalBuilder.Core.Builder;
using TimberLogicalBuilder.Core.Graph;

namespace TimberLogicalBuilder.Components.Components.Display;

// The stackup:
// [Display cube]
// [Memory unit]

public record ZeroBitOutput();

public class Display1(
  string identifier, 
  ISignalSource input, 
  ISignalSource writeEnable, 
  Color? color = null)
   : BaseComponent<ZeroBitOutput>
{
  public override ZeroBitOutput Build(ComponentContext context)
  {
    context.Builder.Layout(context.Position, LayoutAxes.VerticalX, l =>
    {
      // FlipFlop(string name, Vector3Int position, ISignalSource inputA, ISignalSource inputB, ISignalSource? reset = null)
      var mem = l
        .Latch(identifier + "_mem", input, writeEnable)
        .Covered();
      // string name, Vector3Int position, ISignalSource input, Color? color = null
      var disp = l
        .Indicator(identifier + "_i", mem, color);
    });

    return new ZeroBitOutput();
  }
}
