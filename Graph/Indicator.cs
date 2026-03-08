using System.Drawing;
using TimberLogicalBuilder.Core.Model;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.Graph;

public class Indicator(string name, Vector3Int pos) : LogicNode(name, pos)
{
  public ISignalSource? Input { get; private set; }
  public bool IsPinned { get; private set; }
  public Color? CustomColor { get; private set; }

  public Indicator Connect(ISignalSource input)
  {
    Input = input;
    return this;
  }
  
  public Indicator Pinned()
  {
    IsPinned = true;
    return this;
  }
  public Indicator UnPinned()
  {
    IsPinned = false;
    return this;
  }
  
  public Indicator Color(Color color)
  {
    CustomColor = color;
    return this;
  }
}