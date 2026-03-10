namespace TimberLogicalBuilder.Components.ComponentSystem;

public abstract class BaseComponent<TOutput>
{
  public abstract TOutput Build(ComponentContext context);
}