using TimberLogicalBuilder.Core.Model;

namespace TimberLogicalBuilder.Components.Structs;

public readonly struct Word1(ISignalSource b0)
{
  public ISignalSource B0 { get; } = b0;

  public ISignalSource[] Bits => [B0];
  public ISignalSource this[int index] => Bits[index];

  public static Word1 FromArray(ISignalSource[] bits)
  {
    if(bits.Length != 1) throw new ArgumentException();
    return new Word1(bits[0]);
  }
}