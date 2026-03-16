using TimberLogicalBuilder.Components.ComponentSystem;
using TimberLogicalBuilder.Components.Extensions;
using TimberLogicalBuilder.Components.Structs;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Components.DynamicComponents;

public record MultiOr4Out(Word4 Output);

public class MultiOr4(
  string identifier,
  Vector3Int maxSize)
  : BaseDynamicComponent<Word4, MultiOr4Out>(maxSize)
{
  private MultiOr2[] _ors = new MultiOr2[2];
  
  public override MultiOr4Out Build(ComponentContext context)
  {
    var outputs = new MultiOr2Out[2];

    for (var i = 0; i < 2; i++)
    {
      _ors[i] = new MultiOr2($"{identifier}{i}", MaxSize);
      outputs[i] = context.Layout.Component(_ors[i]);
      // context.Layout.Step();
      // context.Layout.Step();
    }
    
    return new MultiOr4Out(Word4.FromWord2(outputs[0].Output, outputs[1].Output));
  }

  public override void ConnectInput(Word4 input)
  {
    var inputs = Word4.ToWord2(input);
    _ors[0].ConnectInput(inputs.low);
    _ors[1].ConnectInput(inputs.high);
  }
}