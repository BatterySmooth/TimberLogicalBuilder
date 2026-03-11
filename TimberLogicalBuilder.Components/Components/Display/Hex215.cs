using System.Diagnostics.SymbolStore;
using System.Drawing;
using TimberLogicalBuilder.Components.ComponentSystem;
using TimberLogicalBuilder.Components.Extensions;
using TimberLogicalBuilder.Components.Structs;
using TimberLogicalBuilder.Core.Graph;
using TimberLogicalBuilder.Core.Model;

namespace TimberLogicalBuilder.Components.Components.Display;

// a map to turn a demuxed sixteen-option input into a fifteen-bit hex out.
// The font:
//
//   ##        ##        ##      ######    ##  ##    ######    ######    ######
// ##  ##    ####      ##  ##        ##    ##  ##    ##        ##            ##
// ##  ##      ##          ##    ######    ######    ######    ######      ##  
// ##  ##      ##        ##          ##        ##        ##    ##  ##      ##  
//   ##      ######    ######    ######        ##    ####      ######    ##    
//
//  ##      ######      ##      ####        ####    ####      ######    ######
//##  ##    ##  ##    ##  ##    ##  ##    ##        ##  ##    ##        ##    
//######    ######    ######    ######    ##        ##  ##    ######    ######
//##  ##        ##    ##  ##    ##  ##    ##        ##  ##    ##        ##    
//  ##      ######    ##  ##    ####        ####    ####      ######    ##    
//
// Yeah, it's rough, but it's also 3x5. Ya gets the pixels ya pays for.

public record Hex215Out(Word15 Channels);

public class Hex215(
  string identifier,
  Word16 input
) : BaseComponent<Hex215Out>
{
  private static int[] bitmap = {
    0b010101101101010, 0b010110010010111, 0b010101001010111, 0b111001111001111, 
    0b101101111001001, 0b111100111001110, 0b111100111101111, 0b111001010010100,
    0b010101111101010, 0b111101111001111, 0b010101111101101, 0b110101111101110, 
    0b011100100100011, 0b110101101101110, 0b111100111100111, 0b111100111100100};

  public override Hex215Out Build(ComponentContext context)
  {
    var layout = context.RequireLayout();
    Relay[,] relays = new Relay[16,15];
    Relay[] lastForBit = new Relay[15];

    // first we generate the matrix, symbol-wise
    for(int bit = 0; bit < 15; bit++)
    {
      for(int sym = 0; sym < 16; sym++)
      {
        if ((bitmap[sym] & (0b100000000000000 >>> bit)) != 0)
        {
          Relay last = lastForBit[bit];

          Relay rel = layout.Or((identifier + "_" + sym + "_" + bit), input[sym], last?? input[sym]);

          relays[sym, bit] = rel;
          lastForBit[bit] = rel;
        }
        else
        {
          layout.Step();
        }
      }
      
      layout.NextRow();
    }

    return new Hex215Out(Word15.FromArray(lastForBit));
  }
}
