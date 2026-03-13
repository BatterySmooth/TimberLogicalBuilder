using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.Model;

public abstract class DualInputNode(string name, Vector3Int pos) : LogicNode<DualInputNode>(name, pos), ISignalSource
{
  public DualInputNode Connect(ISignalSource a, ISignalSource b)
  {
    InputA = a;
    InputB = b;
    return this;
  }
}