using System.Diagnostics.SymbolStore;
using System.Drawing;
using TimberLogicalBuilder.Components.ComponentSystem;
using TimberLogicalBuilder.Components.Extensions;
using TimberLogicalBuilder.Components.Structs;
using TimberLogicalBuilder.Core.Graph;

namespace TimberLogicalBuilder.Components.Components.Display;

// a map to turn a demuxed 128-option input into a fifteen-bit display out.
// The font:
//
// (0x00-0x1F: Unprintable)
// (0x20: space)
//   
//             XX      XX  XX    XX  XX      XXXX    XX          XX        XX  
//             XX      XX  XX    XXXXXX    XXXX          XX    XX  XX      XX  
//             XX                XX  XX      XX        XX        XXXX          
//                               XXXXXX      XXXX    XX        XX  XX          
//             XX                XX  XX    XXXX          XX    XXXXXX          
//
// 0b000000000000000, 0b010010010000010, 0b101101000000000, 0b101111101111101,
// 0b011110010011110, 0b100001010100001, 0b010101011101111, 0b010010000000000,
//
//     XX    XX        XX  XX                                                XX
//   XX        XX        XX        XX                                        XX
//   XX        XX      XX  XX    XXXXXX              XXXXXX                XX  
//   XX        XX                  XX        XX                            XX  
//     XX    XX                            XX                    XX      XX    
// 
// 0b001010010010001, 0b010001001001010, 0b101010101000000, 0b000010111010000,
// 0b000000000010100, 0b000000111000000, 0b000000000000010, 0b001001010010100,
//
// XXXXXX      XX        XX      XXXXXX    XX  XX    XXXXXX    XXXXXX    XXXXXX
// XX  XX    XXXX      XX  XX        XX    XX  XX    XX        XX            XX
// XX  XX      XX          XX    XXXXXX    XXXXXX    XXXXXX    XXXXXX      XX  
// XX  XX      XX        XX          XX        XX        XX    XX  XX      XX  
// XXXXXX    XXXXXX    XXXXXX    XXXXXX        XX    XXXXXX    XXXXXX    XX    
//
// 0b111101101101111, 0b010110010010111, 0b010101001010111, 0b111001111001111,
// 0b101101111001001, 0b111100111001111, 0b111100111101111, 0b111001010010100,
//
// XXXXXX    XXXXXX                            XX              XX          XX  
// XX  XX    XX  XX      XX        XX        XX      XXXXXX      XX      XX  XX
// XXXXXX    XXXXXX                        XX                      XX        XX
// XX  XX        XX      XX        XX        XX      XXXXXX      XX        XX  
// XXXXXX    XXXXXX    XX                      XX              XX          XX  
//
// 0b111101111101111, 0b111101111001111, 0b000010000010100, 0b000010000010000,
// 0b001010100010001, 0b000111000111000, 0b100010001010100, 0b010101001010010,
//
//   XX        XX      XXXX        XXXX    XXXX      XXXXXX    XXXXXX      XXXX
// XX  XX    XX  XX    XX  XX    XX        XX  XX    XX        XX        XX    
// XXXXXX    XXXXXX    XXXXXX    XX        XX  XX    XXXXXX    XXXXXX    XX  XX
// XX  XX    XX  XX    XX  XX    XX        XX  XX    XX        XX        XX  XX
//   XXXX    XX  XX    XXXX        XXXX    XXXX      XXXXXX    XX          XXXX
//
// 0b010101111101011, 0b010101111101101, 0b110101111101110, 0b011100100100011,
// 0b110101101101110, 0b111100111100111, 0b111100111100100, 0b011100101101011,
//
// XX  XX    XXXXXX        XX    XX  XX    XX        XX  XX      XX        XX  
// XX  XX      XX          XX    XXXX      XX        XXXXXX    XX  XX    XX  XX
// XXXXXX      XX          XX    XX        XX        XX  XX    XX  XX    XX  XX
// XX  XX      XX      XX  XX    XXXX      XX        XX  XX    XX  XX    XX  XX
// XX  XX    XXXXXX    XXXXXX    XX  XX    XXXXXX    XX  XX    XX  XX      XX  
//
// 0b101101111101101, 0b111010010010111, 0b001001001101111, 0b101110100110101,
// 0b100100100100111, 0b101111101101101, 0b010101101101101, 0b010101101101010,
//
// XXXX        XX      XXXX        XXXX    XXXXXX    XX  XX    XX  XX    XX  XX
// XX  XX    XX  XX    XX  XX    XX          XX      XX  XX    XX  XX    XX  XX
// XXXX      XX  XX    XXXX        XX        XX      XX  XX    XX  XX    XX  XX
// XX        XXXXXX    XX  XX        XX      XX      XX  XX    XXXXXX    XXXXXX
// XX          XXXX    XX  XX    XXXX        XX        XX        XX      XX  XX
//
// 0b110101110100100, 0b010101101111011, 0b110101110101101, 0b011100010001110,
// 0b111010010010010, 0b101101101101010, 0b101101101111010, 0b101101101111101,
//
// XX  XX    XX  XX    XXXXXX      XXXX    XX        XXXX        XX            
// XX  XX    XX  XX        XX      XX      XX          XX      XX  XX          
//   XX        XX        XX        XX        XX        XX                      
// XX  XX      XX      XX          XX        XX        XX                      
// XX  XX      XX      XXXXXX      XXXX        XX    XXXX                XXXXXX
//
// 0b101101010101101, 0b101101010010010, 0b111001010100111, 0b011010010010011,
// 0b100100010010001, 0b110010010010110, 0b010101000000000, 0b000000000000111,
//
// XX                  XX                      XX                  XX      XX  
//   XX                XX                      XX                XX      XX  XX
//             XXXX    XXXX        XXXX      XXXX      XXXX    XXXXXX      XXXX
//           XX  XX    XX  XX    XX        XX  XX    XXXXXX      XX          XX
//             XXXX    XXXX        XXXX      XXXX    XXXXXX      XX      XXXX  
//
// 0b100010000000000, 0b000000011101011, 0b100100110101110, 0b000000011100011,
// 0b001001011101011, 0b000000011111111, 0b001010111010010, 0b010101011001110,
//
// XX                    XX      XX          XX                                
// XX          XX                XX          XX                                
// XXXX                  XX      XX  XX      XX      XXXXXX      XX        XX  
// XX  XX      XX        XX      XXXX        XX      XXXXXX    XX  XX    XX  XX
// XX  XX      XX      XX        XX  XX      XX      XX  XX    XX  XX      XX  
//
// 0b100100110101101, 0b000010000010010, 0b010000010010100, 0b100100101110101,
// 0b010010010010010, 0b000000111111101, 0b000000010101101, 0b000000010101010,
//
//                                           XX                                
// XXXX        XX                  XXXX      XX                                
// XX  XX    XX  XX      XXXX    XXXX      XXXXXX    XX  XX    XX  XX    XX  XX
// XXXX        XXXX    XX            XX      XX      XX  XX    XX  XX    XXXXXX
// XX            XX    XX        XXXX        XX      XXXXXX      XX      XXXXXX
//
// 0b000110101110100, 0b000010101011001, 0b000000011100100, 0b000011110001110,
// 0b010010111010010, 0b000000101101111, 0b000000101101010, 0b000000101111111,
//
//                                 XXXX      XX      XXXX      
//                     XXXXXX      XX        XX        XX        XXXX
// XX  XX    XX  XX      XX      XXXX        XX        XXXX    XX    
//   XX        XX      XX          XX        XX        XX      
// XX  XX    XX        XXXXXX      XXXX      XX      XXXX      
//
// 0b000000101010101, 0b000000101010100, 0b000111010100111, 0b011010110010011,
// 0b010010010010010, 0b110010011010110, 0b000011100000000, 0b000000000000000
//

public record ASCII215Out(Word15 Channels);

public class ASCII215(
  string identifier,
  ISignalSource[] input
) : BaseComponent<ASCII215Out>
{
  private static int[] bitmap =
  {
    0b000000000000000, 0b010010010000010, 0b101101000000000, 0b101111101111101,
    0b011110010011110, 0b100001010100001, 0b010101011101111, 0b010010000000000,
    0b001010010010001, 0b010001001001010, 0b101010101000000, 0b000010111010000,
    0b000000000010100, 0b000000111000000, 0b000000000000010, 0b001001010010100,
    0b111101101101111, 0b010110010010111, 0b010101001010111, 0b111001111001111,
    0b101101111001001, 0b111100111001111, 0b111100111101111, 0b111001010010100,
    0b111101111101111, 0b111101111001111, 0b000010000010100, 0b000010000010000,
    0b001010100010001, 0b000111000111000, 0b100010001010100, 0b010101001010010,
    0b010101111101011, 0b010101111101101, 0b110101111101110, 0b011100100100011,
    0b110101101101110, 0b111100111100111, 0b111100111100100, 0b011100101101011,
    0b101101111101101, 0b111010010010111, 0b001001001101111, 0b101110100110101,
    0b100100100100111, 0b101111101101101, 0b010101101101101, 0b010101101101010,
    0b110101110100100, 0b010101101111011, 0b110101110101101, 0b011100010001110,
    0b111010010010010, 0b101101101101010, 0b101101101111010, 0b101101101111101,
    0b101101010101101, 0b101101010010010, 0b111001010100111, 0b011010010010011,
    0b100100010010001, 0b110010010010110, 0b010101000000000, 0b000000000000111,
    0b100010000000000, 0b000000011101011, 0b100100110101110, 0b000000011100011,
    0b001001011101011, 0b000000011111111, 0b001010111010010, 0b010101011001110,
    0b100100110101101, 0b000010000010010, 0b010000010010100, 0b100100101110101,
    0b010010010010010, 0b000000111111101, 0b000000010101101, 0b000000010101010,
    0b000110101110100, 0b000010101011001, 0b000000011100100, 0b000011110001110,
    0b010010111010010, 0b000000101101111, 0b000000101101010, 0b000000101111111,
    0b000000101010101, 0b000000101010100, 0b000111010100111, 0b011010110010011,
    0b010010010010010, 0b110010011010110, 0b000011100000000, 0b000000000000000
  };

  public override ASCII215Out Build(ComponentContext context)
  {
    LogicNode[,] relays = new LogicNode[128,15];
    LogicNode[] lastForBit = new LogicNode[15];

    context.Builder.Layout(context.Position, context.Axes, l =>
    {
      // first we generate the matrix, symbol-wise
      for(int bit = 0; bit < 15; bit++)
      {
        for(int sym = 32; sym < 128; sym++)
        {
          if ((bitmap[sym-32] & (0b100000000000000 >>> bit)) != 0)
          {
            LogicNode last = lastForBit[bit];

            LogicNode rel = l.Or((identifier + "_" + sym + "_" + bit), input[sym], last?? input[sym]);

            relays[sym, bit] = rel;
            lastForBit[bit] = rel;
          }
          else
          {
            l.Step();
          }
        }
      
        l.NextRow();
      }
    });
    
    return new ASCII215Out(Word15.FromArray(lastForBit.Cast<ISignalSource>().ToArray()));
  }
}
