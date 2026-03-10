using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.Model;

public abstract class LogicNode(string name, Vector3Int position)
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Name { get; set; } = name;
  public Vector3Int Position { get; set; } = position;
  public virtual bool IsCovered { get; protected set; }
}

public abstract class LogicNode<T>(string name, Vector3Int position) : LogicNode(name, position) where T : LogicNode<T>
{
  public T Covered()
  {
    IsCovered = true;
    return (T)this;
  }
  public T UnCovered()
  {
    IsCovered = false;
    return (T)this;
  }
}