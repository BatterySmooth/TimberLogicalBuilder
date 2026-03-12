using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.Model;

public abstract class LogicNode(string name, Vector3Int position)
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Name { get; set; } = name;
  public Vector3Int Position { get; set; } = position;
  public virtual bool IsCovered { get; protected set; }

  // Okay, so this breaks encapsulation best practices
  // But having these be always present makes ingestion a ton simpler
  public ISignalSource? InputA {get; set;}
  public ISignalSource? InputB {get; set;}
  public ISignalSource? ResetInput {get; set;}
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