using TimberLogicalBuilder.Core.Model;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.Graph;

public class Relay(string name, Vector3Int pos, RelayMode mode) : LogicNode<Relay>(name, pos), ISignalSource
{
  public RelayMode Mode { get; } = mode;

  public ISignalSource? InputA { get; private set; }
  public ISignalSource? InputB { get; private set; }

  public Relay Inputs(ISignalSource a, ISignalSource? b = null)
  {
    InputA = a;
    InputB = b;
    return this;
  }
}