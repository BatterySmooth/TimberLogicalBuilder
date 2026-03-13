using TimberLogicalBuilder.Components.ComponentSystem;
using TimberLogicalBuilder.Core.Graph;

namespace TimberLogicalBuilder.Components;

public record MemoryCellOutput(ISignalSource Q);

public class MemoryCell : BaseComponent<MemoryCellOutput>
{

  private readonly ISignalSource _write;
  private readonly ISignalSource _data;

  public MemoryCell(ISignalSource write, ISignalSource data)
  {
    _write = write;
    _data = data;
  }

  public override MemoryCellOutput Build(ComponentContext ctx)
  {
    var mem = ctx.Layout!.SetReset("cell", _data, _write);

    return new MemoryCellOutput(mem);
  }
  
  // // Example of nested layout usage in component scope:
  // private void NestedLayout(ComponentContext ctx, Vector3Int anchor)
  // {
  //   ctx.Builder.Layout(anchor, LayoutAxis.X, LayoutAxis.Y, 2, layout =>
  //   {
  //     for (int i = 0; i < 8; i++)
  //     {
  //       var cell = layout.Component(() => new MemoryCell(_write, _data[i]));
  //       outputs.Add(cell.Q);
  //       layout.NextRow();
  //     }
  //   });
  // }
}