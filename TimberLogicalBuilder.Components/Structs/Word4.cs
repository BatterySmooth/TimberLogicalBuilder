using TimberLogicalBuilder.Core.Model;

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
}