using TimberLogicalBuilder.Core.Model;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.Graph;

public class Memory(string name, Vector3Int pos, MemoryMode mode) : LogicNode<Memory>(name, pos), ISignalSource
{
  public MemoryMode Mode { get; } = mode;

  public ISignalSource? InputA { get; private set; }
  public ISignalSource? InputB { get; private set; }
  public ISignalSource? ResetInput { get; private set; }

  public Memory Inputs(ISignalSource a, ISignalSource? b = null)
  {
    InputA = a;
    InputB = b;
    return this;
  }

  public Memory Reset(ISignalSource r)
  {
    ResetInput = r;
    return this;
  }
}