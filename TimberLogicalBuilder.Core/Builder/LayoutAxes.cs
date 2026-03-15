using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.Builder;

public readonly struct LayoutAxes(Vector3Int primary, Vector3Int secondary, Vector3Int tertiary)
{
  public Vector3Int Primary { get; } = primary;
  public Vector3Int Secondary { get; } = secondary;
  public Vector3Int Tertiary { get; } = tertiary;
  
  public static implicit operator LayoutAxes((Vector3Int primary,Vector3Int secondary,Vector3Int tertiary) t)
    => new(t.primary, t.secondary, t.tertiary);

  public static LayoutAxes VerticalX => new(
    new Vector3Int(0, 0, 2), 
    new Vector3Int(1, 0, 0), 
    new Vector3Int(0, 1, 0));
  
  public static LayoutAxes VerticalY => new(
    new Vector3Int(0, 0, 2), 
    new Vector3Int(0, 1, 0), 
    new Vector3Int(1, 0, 0));
}