using TimberLogicalBuilder.Core.Graph;

namespace TimberLogicalBuilder.Components.Structs;

public readonly struct Word2(ISignalSource b0, ISignalSource b1)
{
  public ISignalSource B0 { get; } = b0;
  public ISignalSource B1 { get; } = b1;

  public ISignalSource[] Bits => [B0, B1];
  public ISignalSource this[int index] => Bits[index];

  public static Word2 FromArray(ISignalSource[] bits)
  {
    if(bits.Length != 2) throw new ArgumentException();
    return new Word2(bits[0], bits[1]);
  }
}