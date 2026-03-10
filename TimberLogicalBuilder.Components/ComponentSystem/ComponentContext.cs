using TimberLogicalBuilder.Core.Builder;

namespace TimberLogicalBuilder.Components.ComponentSystem;

public sealed class ComponentContext
{
  public LogicBuilder Builder { get; }
  public LogicLayout? Layout { get; }

  internal ComponentContext(LogicBuilder builder)
  {
    Builder = builder;
  }

  internal ComponentContext(LogicLayout layout)
  {
    Builder = layout.Builder;
    Layout = layout;
  }

  public bool HasLayout => Layout != null;

  public LogicLayout RequireLayout()
    => Layout ?? throw new InvalidOperationException("Component requires a layout context.");
  
  public T LayoutScope<T>(Func<LogicBuilder, LogicLayout, T> scope)
  {
    var layout = RequireLayout();
    return scope(Builder, layout);
  }
  
  // public T LayoutScope<T>(
  //   Vector3Int primaryStep,
  //   Vector3Int secondaryStep,
  //   Vector3Int tertiaryStep,
  //   Func<LogicLayout, T> scope)
  // {
  //   var parent = RequireLayout();
  //   var nested = new LogicLayout(
  //     Builder,
  //     parent.Cursor,
  //     primaryStep,
  //     secondaryStep,
  //     tertiaryStep,
  //     autoAdvance: false);
  //   return scope(nested);
  // }
}