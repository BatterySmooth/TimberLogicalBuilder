using TimberLogicalBuilder.Core.Model;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.Graph;

public class Lever(string name, Vector3Int pos, bool spring = false, bool pinned = false) : LogicNode(name, pos), ISignalSource
{
  public bool IsSpringReturn { get; private set; } = spring;
  public bool IsPinned { get; private set; } = pinned;

  public Lever Pinned()
  {
    IsPinned = true;
    return this;
  }
  public Lever UnPinned()
  {
    IsPinned = false;
    return this;
  }
  
  public Lever Sprung()
  {
    IsSpringReturn = true;
    return this;
  }
  public Lever UnSprung()
  {
    IsSpringReturn = true;
    return this;
  }
}