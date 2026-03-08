// Ignore this crap, it was a shabby attempt at 2 am and a few drinks deep - need to redo it
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.Serialization;

public class NodeDto
{
    public Guid Id { get; set; }
    public string Template { get; set; } = null!;
    public ComponentsDto Components { get; set; } = null!;
}

public class ComponentsDto
{
    public NamedEntityDto NamedEntity { get; set; } = null!;
    public BlockObjectDto BlockObject { get; set; } = null!;
    public AutomatorDto? Automator { get; set; }
    public RelayDto? Relay { get; set; }
    public MemoryDto? Memory { get; set; }
    public TimerDto? Timer { get; set; }
    public IndicatorDto? Indicator { get; set; }
    public InventoryDto InventoryConstructionSite { get; set; } = null!;
    public CustomizableIlluminatorDto? CustomizableIlluminator { get; set; }
    public AutomatableDto? Automatable { get; set; }
}

public record struct Vector3IntDto(int X, int Y, int Z);

public class NamedEntityDto { public string EntityName { get; set; } = null!; }
public class BlockObjectDto { public Vector3IntDto Coordinates { get; set; } }
public class AutomatorDto { public string State { get; set; } = null!; }
public class RelayDto
{
    public string Mode { get; set; } = null!;
    public Guid? InputA { get; set; }
    public Guid? InputB { get; set; }
}
public class MemoryDto
{
    public string Mode { get; set; } = null!;
    public Guid? InputA { get; set; }
    public Guid? InputB { get; set; }
    public Guid? ResetInput { get; set; }
}
public class TimerDto
{
    public string Mode { get; set; } = null!;
    public int Counter { get; set; }
    public Guid? Input { get; set; }
    public Guid? ResetInput { get; set; }
    public TimerIntervalDto TimerIntervalA { get; set; } = null!;
    public TimerIntervalDto TimerIntervalB { get; set; } = null!;
}
public class TimerIntervalDto { public TimerInterval Ticks { get; set; } }
public class IndicatorDto { public string? PinnedMode { get; set; } }
public class CustomizableIlluminatorDto
{
    public bool IsCustomized { get; set; }
    public ColorDto CustomColor { get; set; } = null!;
}
public class ColorDto { public float r, g, b, a; }
public class AutomatableDto { public Guid Input { get; set; } }
public class InventoryDto
{
    public StorageDto Storage { get; set; } = null!;
}
public class StorageDto
{
    public List<GoodDto> Goods { get; set; } = new();
}
public class GoodDto { public string Good { get; set; } = null!; public int Amount { get; set; } }