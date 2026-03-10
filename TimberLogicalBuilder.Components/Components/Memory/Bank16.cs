// using TimberLogicalBuilder.Components.ComponentSystem;
// using TimberLogicalBuilder.Components.Extensions;
// using TimberLogicalBuilder.Components.Structs;
// using TimberLogicalBuilder.Core.Model;
//
// namespace TimberLogicalBuilder.Components.Components.Memory;
//
// public record MemoryBankOutput(Register16Output Channels);
//
// public class Bank16(
//   string identifier,
//   Word16 inputs,
//   int memCount = 8,
//   int channelCount = 2)
//   : BaseComponent<MemoryBankOutput>
// {
//   public override MemoryBankOutput Build(ComponentContext context)
//   {
//     var l = context.RequireLayout();
//
//     const int maxWidth = 16;
//
//     var memSelects = new ISignalSource[memCount][];
//     var placedInRow = 0;
//
//     for (var m = 0; m < memCount; m++)
//     {
//       memSelects[m] = new ISignalSource[channelCount];
//
//       for (var c = 0; c < channelCount; c++)
//       {
//         if (placedInRow >= maxWidth)
//         {
//           l.NextRow();
//           placedInRow = 0;
//         }
//
//         memSelects[m][c] = l.Lever($"{identifier} MEM {m} CHAN SEL {c}").Pinned();
//         placedInRow++;
//       }
//     }
//
//     l.NextRow();
//     l.NextRow();
//
//     Register16Output? previous = null;
//
//     for (var m = 0; m < memCount; m++)
//     {
//       var enable = l.Lever($"{identifier} MemEnable{m}").Pinned();
//
//       var output = l.Component(
//         new Register16(
//           $"{identifier}-{m:D2}",
//           enable,
//           memSelects[m],
//           previous,
//           inputs));
//
//       previous = output;
//     }
//
//     l.NextRow();
//
//     return new MemoryBankOutput(previous!.Channels);
//   }
// }