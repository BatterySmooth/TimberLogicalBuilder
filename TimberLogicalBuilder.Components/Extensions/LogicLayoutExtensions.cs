using TimberLogicalBuilder.Components.ComponentSystem;
using TimberLogicalBuilder.Core.Builder;

namespace TimberLogicalBuilder.Components.Extensions;

public static class LogicLayoutExtensions
{
  public static TOutput Component<TOutput>(this LogicLayout layout, BaseComponent<TOutput> component)
  {
    var context = new ComponentContext(layout.Builder, layout.Cursor, layout.Axes);
    var builtComponent = component.Build(context);
    var advance = context.Layout.PrimarySpan(layout.Axes);
    
    Console.WriteLine($"Component advance: {advance} (type: {builtComponent?.GetType()})");
    layout.Step(advance);
    return builtComponent;
  }
}