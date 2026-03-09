# Timber Logical Builder

This library is to help scaffold and build logical systems in Timberborn. The library allows the user to use a fluent API builder to declare the layout of the system, build, and serialise it to an output save.

## Installation

### Prerequisites

- .NET 10 SDK installed
- Basic understanding of C#/.NET
- A save to use as the source

## Usage

### Builder

The Builder is the main class for constructing logical systems. You can create one like so:

```csharp
var builder = new LogicBuilder();
```

### Creating Components

To create a component, you can call various methods on the builder to create components. Each of these function return the component it creates, so you can save this in a variable and use it as inputs in other components

```csharp
var lever = builder.Lever("Lever", (1, 1, 4));
```

Each component takes, at minimum, `name` (string) and `position` (Vector3Int), as displayed above.

> [!TIP]
> The position of components uses a struct from called `Vector3Int`. This has been set up so it can implicitally convert from a 3-integer tuple.
> These 2 return the same thing:
> ```csharp
> var a = new Vector3Int(1, 2, 3);
> var b = (1, 2, 3);
> ```

> [!IMPORTANT]  
> In Timberborn, the Z axis in the upwards axis.

Each component can also be flagged to have a platform built over it using the `Covered()` method, and can be removed using the `UnCovered()` method. This method adds a double platform for all nodes, except for the Indicator, which gets a single platform.

#### Lever

The Lever class has boolean parameters for `IsSpringReturn` and `IsPinned`, and can be set like so:

```csharp
var lever = builder.Lever("Lever", (1, 1, 4))
  .Pinned()
  .Sprung();
```

> [!TIP]
> There are aso methods for `UnPinned()` and `UnSprung()` if you need to un-set one of these properties from a copied component.

#### Relay

The Relay has several modes, and these have been broken down into separate methods on the builder, for ease. A relay can be created with any of these methods:

```csharp
// Used as an example for the sources for the relay components    
var leverA = builder.Lever("Lever A", (0, 0, 0));
var leverB = builder.Lever("Lever B", (1, 0, 0));

var passthrough = builder.Passthrough("Passthrough", (0, 1, 0), leverA);
var not = builder.Not("Not", (1, 1, 0), leverA);
var and = builder.And("And", (2, 1, 0), leverA, leverB);
var or = builder.Or("Or", (3, 1, 0), leverA, leverB);
var xor = builder.Xor("Xor", (4, 1, 0), leverA, leverB);
```

#### Memory

Memory cells are done in a similar matter, with different builder methods for each type.

```csharp
// Used as an example for the sources for the memory components    
var leverA = builder.Lever("Lever A", (0, 0, 0));
var leverB = builder.Lever("Lever B", (1, 0, 0));

var setReset = builder.SetReset("Set Reset", (0, 1, 0), leverA);
var toggle = builder.Toggle("Toggle", (1, 1, 0), leverB);
var latch = builder.Latch("Latch", (2, 1, 0), leverA, leverB);
var flipFlop = builder.FlipFlop("Flip-Flop", (3, 1, 0), leverA, leverB);
```

> [!TIP]
> A Memory component can also optionally take a reset signal source, which can be included as an extra parameter at the end of the builder method.
> ```csharp
> var leverA = builder.Lever("Lever A", (0, 0, 0));
> var leverReset = builder.Lever("Lever Reset", (1, 0, 0));
> var setReset = builder.SetReset("Set Reset", (0, 1, 0), leverA, leverReset);
> ```

#### Timer

The timer component follows the same pattern as Memory components, including the optional reset input (which are excluded from the example below).
Timers also take 1 or 2 `TimerInterval` structs, dependent on type.

The `TimerInterval` struct, like the `Vector3Int` struct, can implicitally convert from a tuple of an `int` and a `TimerUnit` enum value. The examples below produce the same `TimerInterval`.

```csharp
var a = new TimerInterval(1, TimerUnit.Ticks);
var b = (1, TimerUnit.Ticks);
```

The various Timer components can be created like so:

```csharp
// Used as an example for the sources for the timer components    
var leverA = builder.Lever("Lever A", (0, 0, 0));

var pulse = builder.Pulse("Pulse", (0, 1, 0), (1, TimerUnit.Ticks), leverA);
var accumulator = builder.Accumulator("Accumulator", (1, 1, 0), (1, TimerUnit.Ticks), leverA);
var delay = builder.Delay("Delay", (2, 1, 0), (1, TimerUnit.Ticks), (1, TimerUnit.Ticks), leverA);
var oscillator = builder.Oscillator("Oscillator", (3, 1, 0), (1, TimerUnit.Ticks), (1, TimerUnit.Ticks), leverA);
```

#### Indicators

Indicators derive the same base parameters as the rest of the logical components liek `name`, `position`, and a single `input`, but can also optionally take a `color` (from the namespace `System.Drawing.Colors`).

```csharp
// Used as an example for the sources for the indicator component    
var leverA = builder.Lever("Lever A", (0, 0, 0));

// Uncoloured indicator
var indicator = builder.Indicator("Indicator", (0, 1, 0), leverA);

// Coloured indicator
var colouredIndicator = builder.Indicator("Coloured Indicator", (1, 1, 0), leverA, Color.DeepPink);
```

### Builder Layout Context

The Builder also contains a method to declare a layout context. A layout context starts from an initial position, and every time a component is created it takes a step along the primary axis, allowing components to be created as a group without declaring position.

Layout contexts can also be nested to allow 2-dimensional or 3-dimensional layouts.

A layout context comes in 2 forms:

- Basic, which takes in:
  - A starting position, called the `anchor`
  - A primary axis, from the enum `LayoutAxis` (any of `LayoutAxis.X`, `LayoutAxis.Y`, or `LayoutAxis.Z`)
  - A secondary axis, in the same form as the primary axis
  - A spacing, controlling the number of blocks moved along the primary axis
- Advanced, which takes in:
  - A starting position, called the `anchor`
  - A `Vector3Int` of the primary step
  - A `Vector3Int` of the secondary step
  - A `Vector3Int` of the tertiary step

When inside the layout context, after the creation of each component the cursor position moves by the step declared in the primary step. In the basic usage, this means taking a step along the primary axis. To move the cursor along the secondary axis, you can call the `NextRow()` method, which optionally takes in a boolean to control whether the primary axis position is reset - set to `true` by default. To advance along the tertiary direction, call the `NextLayer()` method. This also optionally takes in 2 booleans to control whether to reset the primary and secondary axis - once again these default to `true`.

A basic use of the layout context would be to create a line of 8 indicators:

```csharp
// Input lever for the builder layout example    
var source = builder.Lever("Source", (1, 1, BaseZ));
    
var line = builder.Layout((5, 5, BaseZ), LayoutAxis.X, 1, l =>
{
  for (var x = 0; x < 8; x++)
  {
    l.Indicator($"LED_{x}", source);
  }
});
```

This can then be expanded to generate an 8x8 grid of indicators:

```csharp
// Input lever for the builder layout example    
var source = builder.Lever("Source", (1, 1, BaseZ));
    
var grid = builder.Layout((5, 5, BaseZ), LayoutAxis.X, 1, l =>
{
  for (var y = 0; y < 8; y++)
  {
    for (var x = 0; x < 8; x++)
    {
      l.Indicator($"LED_{x}_{y}", source);
    }
    l.NextRow();
  }
});
```

### Structures

For the sake of this document, we will refer to a group of logical components as a Structure.

The builder does not contain any structures out-of-the-box, but has been built with this in mind. You can create structures easily with custom functions, allowing for reusable parts to be defined. This makes it easy for you to drop in a multiplexer or memory bank.

Using the indicator grid above as an example, we can create a structure for it like so:

```csharp
private static void BuildIndicatorGrid(LogicBuilder builder, Vector3Int anchor, ISignalSource input)
{
  builder.Layout(anchor, LayoutAxis.X, 1, l =>
  {
    for (var y = 0; y < 8; y++)
    {
      for (var x = 0; x < 8; x++)
      {
        l.Indicator($"LED_{x}_{y}", input);
      }
      l.NextRow();
    }
  });
}
```

If you need to use components within the structure, you can optionally return them like so:

```csharp
private static List<Indicator> BuildIndicatorGrid(LogicBuilder builder, Vector3Int anchor, ISignalSource input)
{
  var indicators = new List<Indicator>();
  
  builder.Layout(anchor, LayoutAxis.X, 1, l =>
  {
    for (var y = 0; y < 8; y++)
    {
      for (var x = 0; x < 8; x++)
      {
        indicators.Add(l.Indicator($"LED_{x}_{y}", input));
      }
      l.NextRow();
    }
  });
  
  return indicators;
}
```

#### Advanced Srtructure Example

Using this pattern, the builder can be nested to create a complex component. This does not serve any real advantage compared to building the individual components, but allows for them to be built in-line:

```cs
private static ISignalSource BuildClock(LogicBuilder builder, Vector3Int anchor)
{
  var clockAutoLev = builder
    .Lever("Clock (Auto)", anchor + (2, 0, 0))
    .Pinned();
  var clockManOut = builder
    .And("CLK-MAN", anchor + (0, 1, 0), 
      builder
        .Not("!Clock (Auto)", anchor + (1, 1, 0), clockAutoLev)
        .Covered(), 
      builder
        .Lever("Clock (Manual)", anchor).Sprung()
        .Pinned())
    .Covered();
  var clock = builder
    .Or("CLK", anchor + (1, 0, 0), 
      builder
        .Oscillator("CLK-OSCILLATOR", anchor + (2, 1, 0), (1, TimerUnit.Ticks), (1, TimerUnit.Ticks), clockAutoLev)
        .Covered(),
      clockManOut)
    .Covered();
  builder
    .Indicator("Clock", anchor + (1, 0, 2),
      builder.Or("CLK", anchor + (1, 0, 0),
          builder
            .Oscillator("CLK-OSCILLATOR", anchor + (2, 1, 0), (1, TimerUnit.Ticks), (1, TimerUnit.Ticks), clockAutoLev)
            .Covered(),
          clockManOut)
      .Covered(), 
    Color.DeepPink)
    .Pinned();
  return clock;
}
```

## Generating the Output

Once you've constructed the components and structures you want, you can serialise them and output them directly to a new save like so:

```csharp
string InputSave = @"C:\Path\To\Save\Folder\Blank.timber";
string OutputSave = @"C:\Path\To\Save\Folder\GeneratedSave.timber";

var graph = builder.Build();
var output = LogicGraphSerializer.Serialize(graph);
TimberSaveWriter.WriteEntities(InputSave, OutputSave, output);
```

The input save is the save to add the components to and the output save is the new save to be created. The new save is created in the same settlement.

> [!CAUTION]
> Although it's technically possible to build components for a normal save game, it will be significantly easier to use a dedicated superflat-style map with no pre-existing buildings.
> One example would be [this dedicated superflat map](https://steamcommunity.com/sharedfiles/filedetails/?id=3681158472)
