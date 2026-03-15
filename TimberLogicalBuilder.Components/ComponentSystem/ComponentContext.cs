using TimberLogicalBuilder.Core.Builder;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Components.ComponentSystem;

public sealed class ComponentContext
{
  public LogicBuilder Builder { get; }
  public LogicLayout Layout { get; private set; } = null!;
  public Vector3Int Position { get; }
  public LayoutAxes Axes { get; }

  internal ComponentContext(LogicBuilder builder, Vector3Int position, LayoutAxes axes)
  {
    Builder = builder;
    Position = position;
    Axes = axes;
    builder.Layout(position, axes, layout => Layout = layout);
  }
}