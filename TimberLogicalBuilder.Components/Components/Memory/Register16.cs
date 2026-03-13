using TimberLogicalBuilder.Components.ComponentSystem;
using TimberLogicalBuilder.Components.Extensions;
using TimberLogicalBuilder.Components.Structs;
using TimberLogicalBuilder.Core.Graph;

namespace TimberLogicalBuilder.Components.Components.Memory;

public record Register16Output(Word16[] Channels);

public class Register16(
  string registerIdentifier,
  ISignalSource writeEnable,
  ISignalSource[] channelSelects,
  Register16Output? channelBus,
  Word16 input,
  ISignalSource? reset = null)
  : BaseComponent<Register16Output>
{
  public override Register16Output Build(ComponentContext context)
  {
    
    var channelCount = channelSelects.Length;
    var outputs = new ISignalSource[16][];
    
    context.Builder.Layout(context.Position, context.Axes, l =>
    {
      for (var bit = 0; bit < 16; bit++)
      {
        outputs[bit] = l.Component(
            new Register1(
              $"{registerIdentifier}-{bit:X}",
              writeEnable,
              channelSelects,
              ExtractBitPlane(channelBus, bit, channelCount),
              input[bit],
              reset: reset))
          .Channels;
      }
      l.NextRow();
    });
    
    return new Register16Output(Word16.PackBitPlanes(outputs));
  }

  private static Register1Output? ExtractBitPlane(Register16Output? source, int bit, int channelCount)
  {
    if (source == null) return null;
    var result = new ISignalSource[channelCount];
    for (var i = 0; i < channelCount; i++)
      result[i] = source.Channels[i][bit];
    return new Register1Output(result);
  }
}