using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.Model;

public abstract class DualInputNode(string name, Vector3Int pos) : LogicNode<DualInputNode>(name, pos), ISignalSource
{
  public ISignalSource? InputA { get; private set; }
  public ISignalSource? InputB { get; private set; }

  public DualInputNode Connect(ISignalSource a, ISignalSource b)
  {
    InputA = a;
    InputB = b;
    return this;
  }
}