using TimberLogicalBuilder.Core.Model;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.Graph;

public class Empty(string name, Vector3Int pos) : LogicNode(name, pos)
{
  public override bool IsCovered
  {
    get => true;
    protected set { }
  }
}