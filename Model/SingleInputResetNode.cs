using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.Model;

public abstract class SingleInputResetNode(string name, Vector3Int pos) : SingleInputNode(name, pos), IResettable
{
  public ISignalSource? ResetInput { get; private set; }

  public SingleInputResetNode Reset(ISignalSource input)
  {
    ResetInput = input;
    return this;
  }
}