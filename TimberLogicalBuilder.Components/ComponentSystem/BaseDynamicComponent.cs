using TimberLogicalBuilder.Components.Exceptions;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Components.ComponentSystem;

public abstract class BaseDynamicComponent<TInput, TOutput>(Vector3Int maxSize) : BaseComponent<TOutput>
{
  public override bool IsDynamic => true;
  // public override Vector3Int LayoutSize => MaxSize;
  public Vector3Int MaxSize { get; } = maxSize;
  private int _primary;
  private int _secondary;
  private int _tertiary;

  public abstract void ConnectInput(TInput input);
  
  protected void Advance(ComponentContext context)
  {
    _primary++;
    if (_primary < MaxSize.X) return;
    
    // hit primary limit, next row
    _primary = 0;
    _secondary++;
    context.Layout.NextRow();

    if (_secondary < MaxSize.Y) return;

    // hit secondary limit, next layer
    _secondary = 0;
    _tertiary++;
    context.Layout.NextLayer();

    if (_tertiary * 2 >= MaxSize.Z)
      throw new DynamicComponentOverflowException($"Dynamic component exceeded max size {MaxSize}");
  }
}