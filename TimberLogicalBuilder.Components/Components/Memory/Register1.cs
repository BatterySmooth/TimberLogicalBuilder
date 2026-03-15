using TimberLogicalBuilder.Components.ComponentSystem;
using TimberLogicalBuilder.Core.Builder;
using TimberLogicalBuilder.Core.Graph;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Components.Components.Memory;

public record Register1Output(ISignalSource[] Channels);

public class Register1(
  string cellIdentifier,
  ISignalSource writeEnable,
  ISignalSource[] channelSelects,
  ISignalSource input,
  Register1Output? channelBus = null,
  Vector3Int? anchor = null,
  ISignalSource? reset = null)
  : BaseComponent<Register1Output>
{
  public override Register1Output Build(ComponentContext context)
  {
    var outputs = new LogicNode[channelSelects.Length];
    var selects = new LogicNode[channelSelects.Length];
    
    context.Builder.Layout(context.Position, LayoutAxis.Z, LayoutAxis.X, 1, l =>
    {
      for (var i = 0; i < channelSelects.Length; i++)
      {
        selects[i] = l.And($"MEM-{cellIdentifier}-CHAN{i}-SEL")
          .ConnectA(channelSelects[i])
          .Covered();
        
        if (channelBus?.Channels[i] != null)
        {
          outputs[i] = l.Or(
              $"MEM-{cellIdentifier}-CHAN{i}-OUT")
            .ConnectA(selects[i])
            .ConnectB(channelBus.Channels[i])
            .Covered();
        }
        else
        {
          l.Empty($"MEM-{cellIdentifier}-CHAN{i}");
          // When it's the first cell, pass the AND gate as the bus output
          outputs[i] = selects[i];
        }
      }

      var mem = l.FlipFlop($"MEM-{cellIdentifier}")
        .ConnectA(input)
        .ConnectB(writeEnable);
      
      foreach (var select in selects)
      {
        select.ConnectB(mem);
      }
    });
    
    return new Register1Output(outputs.Cast<ISignalSource>().ToArray());
  }
}