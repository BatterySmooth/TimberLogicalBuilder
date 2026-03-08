using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.Model;

public abstract class SingleInputNode(string name, Vector3Int pos) : LogicNode<SingleInputNode>(name, pos), ISignalSource
{
  public ISignalSource? InputA { get; private set; }

  public SingleInputNode Connect(ISignalSource input)
  {
    InputA = input;
    return this;
  }
}