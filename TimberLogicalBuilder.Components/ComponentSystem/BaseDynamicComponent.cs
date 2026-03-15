using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Components.ComponentSystem;

public abstract class BaseDynamicComponent<TOutput>(Vector3Int maxSize) : BaseComponent<TOutput>
{
  public Vector3Int MaxSize { get; } = maxSize;
}