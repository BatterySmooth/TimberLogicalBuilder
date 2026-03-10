using TimberLogicalBuilder.Components.ComponentSystem;
using TimberLogicalBuilder.Core.Builder;

namespace TimberLogicalBuilder.Components.Extensions;

public static class LogicLayoutExtensions
{
  public static TOutput Component<TOutput>(this LogicLayout layout, BaseComponent<TOutput> component)
  {
    var ctx = new ComponentContext(layout);
    return component.Build(ctx);
  }
}