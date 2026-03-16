using TimberLogicalBuilder.Components.ComponentSystem;
using TimberLogicalBuilder.Core.Graph;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Components.DynamicComponents;

public record MultiOr1Out(ISignalSource Output);

public class MultiOr1(
  string identifier,
  Vector3Int maxSize)
  : BaseDynamicComponent<ISignalSource, MultiOr1Out>(maxSize)
{
  private ComponentContext _context = null!;
  private LogicNode _previousNode = null!;
  private ISignalSource? _previousConnection;
  private bool _bIsConnected;
  private int _nodeCounter;
  
  public override MultiOr1Out Build(ComponentContext context)
  {
    _context = context;
    _previousNode = context.Layout.Or($"{identifier}-{_nodeCounter}").Covered();
    return new MultiOr1Out(_previousNode);
  }

  public override void ConnectInput(ISignalSource input)
  {
    if (_previousConnection is null)
    {
      _previousNode.ConnectA(input);
    }
    else if (!_bIsConnected)
    {
      _previousNode.ConnectB(input);
      _bIsConnected = true;
    }
    else
    {
      // Move dynamic layout cursor
      Advance(_context);
      
      _nodeCounter++;
      
      var next = _context.Layout.Or($"{identifier}-{_nodeCounter}")
        .Covered()
        .ConnectA(_previousConnection)
        .ConnectB(input);
      _previousNode.DisconnectB();
      _previousNode.ConnectB(next);
      _previousNode = next;
    }
    
    _previousConnection = input;
  }
}