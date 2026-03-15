using TimberLogicalBuilder.Components.ComponentSystem;
using TimberLogicalBuilder.Core.Builder;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Components.Extensions;

public static class LogicBuilderExtensions
{
  public static TOutput Component<TOutput>(this LogicBuilder builder, Vector3Int position, LayoutAxes axes, BaseComponent<TOutput> component)
  {
    var context = new ComponentContext(builder, position, axes);
    return component.Build(context);
  }
}