using TimberLogicalBuilder.Core.Graph;

namespace TimberLogicalBuilder.Components.Structs;

public readonly struct Word8(
  ISignalSource b0, ISignalSource b1, ISignalSource b2, ISignalSource b3,
  ISignalSource b4, ISignalSource b5, ISignalSource b6, ISignalSource b7)
{
  public ISignalSource B0 { get; } = b0;
  public ISignalSource B1 { get; } = b1;
  public ISignalSource B2 { get; } = b2;
  public ISignalSource B3 { get; } = b3;
  public ISignalSource B4 { get; } = b4;
  public ISignalSource B5 { get; } = b5;
  public ISignalSource B6 { get; } = b6;
  public ISignalSource B7 { get; } = b7;

  public ISignalSource[] Bits => [B0, B1, B2, B3, B4, B5, B6, B7];
  public ISignalSource this[int index] => Bits[index];

  public static Word8 FromArray(ISignalSource[] bits)
  {
    if(bits.Length != 8) throw new ArgumentException();
    return new Word8(bits[0], bits[1], bits[2], bits[3], bits[4], bits[5], bits[6], bits[7]);
  }
  
  public static Word8 FromWord4(Word4 low, Word4 high)
  {
    return new Word8(low.B0, low.B1, low.B2, low.B3, high.B0, high.B1, high.B2, high.B3);
  }
  
  public static (Word4 low, Word4 high) ToWord4(Word8 word8)
  {
    var low = new Word4(word8.B0, word8.B1, word8.B2, word8.B3);
    var high = new Word4(word8.B4, word8.B5, word8.B6, word8.B7);
    return (low, high);
  }
}