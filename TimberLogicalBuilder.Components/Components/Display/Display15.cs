using System.Drawing;
using TimberLogicalBuilder.Components.ComponentSystem;
using TimberLogicalBuilder.Components.Extensions;
using TimberLogicalBuilder.Components.Structs;
using TimberLogicalBuilder.Core.Model;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Components.Components.Display;

// A 15-grid display
// Remembers what you wrote to it last
// Bit order:
// [0][1][2]
// [3][4][5]
// [6][7][8]
// [9][A][B]
// [C][D][E]

// The stackup:
// [Display cube]
// [Memory unit]

public class Display15(
  string identifier, 
  Word15 input, 
  ISignalSource writeEnable, 
  Color? color = null)
   : BaseComponent<zeroBitOutput>
{
  public override zeroBitOutput Build(ComponentContext context)
  {
    var layout = context.RequireLayout();

    Vector3Int primaryDir = layout.PrimaryStep;
    Vector3Int secondaryDir = layout.SecondaryStep;

    // So we're just going to assume that you aren't doing anything silly and trying to skew displays with odd steps.

    bool mirror = !(
      primaryDir.X > 0 && secondaryDir.Y > 0 ||
      primaryDir.Y > 0 && secondaryDir.X < 0 ||
      primaryDir.X < 0 && secondaryDir.Y < 0 ||
      primaryDir.Y < 0 && secondaryDir.X > 0
    );

    if(color == null)
    {
      color = Color.FromArgb(0x85, 0xBD, 0xCC);
    }

    for(int u = (mirror)? 2 : 0; (mirror) ? (u > -1) : (u < 3); u += (mirror) ? -1 : 1)
    {
      for(int v = 0; v < 5; v++)
      {
      layout.Component(
        new Display1(
        identifier + "_" + v + "_" + u,
        input[v*3+u],
        writeEnable,
        color
        )
      );

      layout.Step();
      }

      layout.NextRow();
    }

    return new zeroBitOutput();
  }

}