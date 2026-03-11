using System.Drawing;
using TimberLogicalBuilder.Components.ComponentSystem;
using TimberLogicalBuilder.Components.Extensions;
using TimberLogicalBuilder.Components.Structs;
using TimberLogicalBuilder.Core.Model;

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

    if(color == null)
    {
      color = Color.FromArgb(0x85, 0xBD, 0xCC);
    }

    for(int u = 0; u < 3; u++)
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