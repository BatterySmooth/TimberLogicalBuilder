using TimberLogicalBuilder.Components.ComponentSystem;
using TimberLogicalBuilder.Components.Extensions;
using TimberLogicalBuilder.Components.Structs;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Components.DynamicComponents;

public record MultiOr2Out(Word2 Output);

public class MultiOr2(
  string identifier,
  Vector3Int maxSize)
  : BaseDynamicComponent<Word2, MultiOr2Out>(maxSize)
{
  private MultiOr1[] _ors = new MultiOr1[2];
  
  public override MultiOr2Out Build(ComponentContext context)
  {
    var outputs = new MultiOr1Out[2];

    for (var i = 0; i < 2; i++)
    {
      _ors[i] = new MultiOr1($"{identifier}{i}", MaxSize);
      outputs[i] = context.Layout.Component(_ors[i]);
      // context.Layout.Step();
    }
    
    return new MultiOr2Out(new Word2(outputs[0].Output, outputs[1].Output));
  }

  public override void ConnectInput(Word2 input)
  {
    var inputs = input.Bits;
    for (var i = 0; i < 2; i++)
      _ors[i].ConnectInput(inputs[i]);
  }
}