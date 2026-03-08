namespace TimberLogicalBuilder.Core.Structs;

public readonly record struct Vector3Int(int X, int Y, int Z)
{
  public static implicit operator Vector3Int((int x,int y,int z) t) => new Vector3Int(t.x, t.y, t.z);
  
  public static Vector3Int operator +(Vector3Int a, Vector3Int b)
    => new Vector3Int(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

  public static Vector3Int operator -(Vector3Int a, Vector3Int b)
    => new Vector3Int(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

  public static Vector3Int operator *(Vector3Int v, int s)
    => new Vector3Int(v.X * s, v.Y * s, v.Z * s);

  public static Vector3Int operator /(Vector3Int v, int s)
    => new Vector3Int(v.X / s, v.Y / s, v.Z / s);
}