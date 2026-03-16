using TimberLogicalBuilder.Core.Graph;

namespace TimberLogicalBuilder.Components.Structs;

public readonly struct Word4(ISignalSource b0, ISignalSource b1, ISignalSource b2, ISignalSource b3)
{
  public ISignalSource B0 { get; } = b0;
  public ISignalSource B1 { get; } = b1;
  public ISignalSource B2 { get; } = b2;
  public ISignalSource B3 { get; } = b3;

  public ISignalSource[] Bits => [B0, B1, B2, B3];
  public ISignalSource this[int index] => Bits[index];

  public static Word4 FromArray(ISignalSource[] bits)
  {
    if(bits.Length != 4) throw new ArgumentException();
    return new Word4(bits[0], bits[1], bits[2], bits[3]);
  }
  
  public static Word4 FromWord2(Word2 low, Word2 high)
  {
    return new Word4(low.B0, low.B1, high.B0, high.B1);
  }
  
  public static (Word2 low, Word2 high) ToWord2(Word4 word4)
  {
    var low = new Word2(word4.B0, word4.B1);
    var high = new Word2(word4.B2, word4.B3);
    return (low, high);
  }
}