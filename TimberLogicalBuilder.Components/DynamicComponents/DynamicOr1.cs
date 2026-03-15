using TimberLogicalBuilder.Components.ComponentSystem;
using TimberLogicalBuilder.Core.Graph;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Components.DynamicComponents;

public record DynamicOr1Out(ISignalSource Output);

public class DynamicOr1(Vector3Int maxSize) : BaseDynamicComponent<ISignalSource, DynamicOr1Out>(maxSize)
{
  public override DynamicOr1Out Build(ComponentContext context)
  {
    throw new NotImplementedException();
  }

  public override void ConnectInput(ISignalSource input)
  {
    throw new NotImplementedException();
  }
}