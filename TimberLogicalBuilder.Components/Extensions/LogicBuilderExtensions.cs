using TimberLogicalBuilder.Components.ComponentSystem;
using TimberLogicalBuilder.Core.Builder;

namespace TimberLogicalBuilder.Components.Extensions;

public static class LogicBuilderExtensions
{
  public static TOutput Component<TOutput>(this LogicBuilder builder, BaseComponent<TOutput> component)
  {
    var ctx = new ComponentContext(builder);
    return component.Build(ctx);
  }
}