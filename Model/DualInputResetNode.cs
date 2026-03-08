using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.Model;

public abstract class DualInputResetNode(string name, Vector3Int pos) : DualInputNode(name, pos), IResettable
{
  public ISignalSource? ResetInput { get; private set; }

  public DualInputResetNode Reset(ISignalSource input)
  {
    ResetInput = input;
    return this;
  }
}