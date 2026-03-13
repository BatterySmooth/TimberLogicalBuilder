namespace TimberLogicalBuilder.Core.Graph;

public class LogicGraph
{
  private readonly List<LogicNode> _nodes = [];

  public IReadOnlyList<LogicNode> Nodes => _nodes;

  internal T Add<T>(T node) where T : LogicNode
  {
    _nodes.Add(node);
    return node;
  }
}