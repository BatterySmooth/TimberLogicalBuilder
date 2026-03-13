using TimberLogicalBuilder.Core.Graph;

namespace TimberLogicalBuilder.Components.Structs;

public readonly struct Word16(
  ISignalSource b0,
  ISignalSource b1,
  ISignalSource b2,
  ISignalSource b3,
  ISignalSource b4,
  ISignalSource b5,
  ISignalSource b6,
  ISignalSource b7,
  ISignalSource b8,
  ISignalSource b9,
  ISignalSource b10,
  ISignalSource b11,
  ISignalSource b12,
  ISignalSource b13,
  ISignalSource b14,
  ISignalSource b15)
{
  public ISignalSource B0 { get; } = b0;
  public ISignalSource B1 { get; } = b1;
  public ISignalSource B2 { get; } = b2;
  public ISignalSource B3 { get; } = b3;
  public ISignalSource B4 { get; } = b4;
  public ISignalSource B5 { get; } = b5;
  public ISignalSource B6 { get; } = b6;
  public ISignalSource B7 { get; } = b7;
  public ISignalSource B8 { get; } = b8;
  public ISignalSource B9 { get; } = b9;
  public ISignalSource B10 { get; } = b10;
  public ISignalSource B11 { get; } = b11;
  public ISignalSource B12 { get; } = b12;
  public ISignalSource B13 { get; } = b13;
  public ISignalSource B14 { get; } = b14;
  public ISignalSource B15 { get; } = b15;

  public ISignalSource[] Bits =>
  [
    B0, B1, B2,  B3,  B4,  B5,  B6,  B7,
    B8, B9, B10, B11, B12, B13, B14, B15
  ];
  public ISignalSource this[int index] => Bits[index];
  
  public static Word16 FromArray(ISignalSource[] bits)
  {
    if(bits.Length != 16) throw new ArgumentException();
    return new Word16(
      bits[0], bits[1], bits[2],  bits[3],  bits[4],  bits[5],  bits[6],  bits[7],
      bits[8], bits[9], bits[10], bits[11], bits[12], bits[13], bits[14], bits[15]);
  }
  
  public static ISignalSource[] FlattenBitPlanes(Word16[] words)
  {
    var wordCount = words.Length;
    var result = new ISignalSource[wordCount * 16];
    for (var bit = 0; bit < 16; bit++)
    for (var w = 0; w < wordCount; w++)
      result[bit * wordCount + w] = words[w][bit];
    return result;
  }
  
  public static ISignalSource[][] SplitBitPlanes(Word16[] words)
  {
    var wordCount = words.Length;
    var bitCount = 16;
    var bitPlanes = new ISignalSource[bitCount][];
    for (var bit = 0; bit < bitCount; bit++)
      bitPlanes[bit] = new ISignalSource[wordCount];
    for (var w = 0; w < wordCount; w++)
    for (var bit = 0; bit < bitCount; bit++)
      bitPlanes[bit][w] = words[w][bit];
    return bitPlanes;
  }
  
  public static ISignalSource?[][] SplitNullableBitPlanes(Word16?[] words)
  {
    var wordCount = words.Length;
    var bitCount = 16;
    var bitPlanes = new ISignalSource?[bitCount][];
    for (var bit = 0; bit < bitCount; bit++)
      bitPlanes[bit] = new ISignalSource?[wordCount];
    for (var w = 0; w < wordCount; w++)
    for (var bit = 0; bit < bitCount; bit++)
      bitPlanes[bit][w] = words[w]?[bit];
    return bitPlanes;
  }
  
  public static Word16[] PackBitPlanes(ISignalSource[][] bitPlanes)
  {
    if (bitPlanes.Length != 16)
      throw new ArgumentException("Expected 16 bit planes for Word16.");
    var wordCount = bitPlanes[0].Length;
    var words = new Word16[wordCount];
    for (var i = 0; i < wordCount; i++)
    {
      words[i] = new Word16(
        bitPlanes[0][i],
        bitPlanes[1][i],
        bitPlanes[2][i],
        bitPlanes[3][i],
        bitPlanes[4][i],
        bitPlanes[5][i],
        bitPlanes[6][i],
        bitPlanes[7][i],
        bitPlanes[8][i],
        bitPlanes[9][i],
        bitPlanes[10][i],
        bitPlanes[11][i],
        bitPlanes[12][i],
        bitPlanes[13][i],
        bitPlanes[14][i],
        bitPlanes[15][i]
      );
    }
    return words;
  }
}
