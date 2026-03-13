using TimberLogicalBuilder.Components.ComponentSystem;
using TimberLogicalBuilder.Core.Graph;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Components.Components.Memory;

public record Register1Output(ISignalSource[] Channels);

public class Register1(
  string cellIdentifier,
  ISignalSource writeEnable,
  ISignalSource[] channelSelects,
  Register1Output? channelBus,
  ISignalSource input,
  Vector3Int? anchor = null,
  ISignalSource? reset = null)
  : BaseComponent<Register1Output>
{
  public override Register1Output Build(ComponentContext context)
  {
    var localCursor = anchor ?? context.RequireLayout().Cursor;
      
    var mem = context.Builder.FlipFlop(
      $"MEM-{cellIdentifier}",
      localCursor + (0, 0, 4 * channelSelects.Length),
      input,
      writeEnable,
      reset);

    var outputs = new ISignalSource[channelSelects.Length];

    for (var i = 0; i < channelSelects.Length; i++)
    {
      var channelSelect = context.Builder.And(
          $"MEM-{cellIdentifier}-CHAN{i}-SEL",
          localCursor,
          mem,
          channelSelects[i])
        .Covered();
      
      localCursor += (0, 0, 2);

      if (channelBus?.Channels[i] != null)
      {
        outputs[i] = context.Builder.Or(
          $"MEM-{cellIdentifier}-CHAN{i}-OUT",
          localCursor,
          channelSelect,
          channelBus.Channels[i])
          .Covered();
      }
      else
      {
        context.Builder.Empty($"MEM-{cellIdentifier}-CHAN{i}", localCursor);
        // When it's the first cell, pass the AND gate as the bus output
        outputs[i] = channelSelect;
      }
      
      localCursor += (0, 0, 2);
    }

    context.RequireLayout().Step();
    return new Register1Output(outputs);
  }
}