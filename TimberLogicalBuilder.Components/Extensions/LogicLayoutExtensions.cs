using TimberLogicalBuilder.Components.ComponentSystem;
using TimberLogicalBuilder.Core.Builder;

namespace TimberLogicalBuilder.Components.Extensions;

public static class LogicLayoutExtensions
{
  public static TOutput Component<TOutput>(this LogicLayout layout, BaseComponent<TOutput> component)
  {
    var context = new ComponentContext(layout.Builder, layout.Cursor, layout.Axes);
    return component.Build(context);
  }
}