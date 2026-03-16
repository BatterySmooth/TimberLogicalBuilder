using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Components.ComponentSystem;

public abstract class BaseComponent<TOutput>
{
  public virtual bool IsDynamic => false;
  // public abstract Vector3Int LayoutSize { get; }
  public abstract TOutput Build(ComponentContext context);
}