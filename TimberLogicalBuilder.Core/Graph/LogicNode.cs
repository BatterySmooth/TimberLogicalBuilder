using System.Drawing;
using TimberLogicalBuilder.Core.Exceptions;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.Graph;

public class LogicNode(NodeType type, string name, Vector3Int position) : ISignalSource
{
  public NodeType NodeType {get; set; } = type;
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Name { get; set; } = name;
  public Vector3Int Position { get; set; } = position;
  
  public bool IsEmpty()
  {
    return NodeType == NodeType.Empty;
  }
  
  public RelayMode? RelayMode { get; private set; }
  public LogicNode SetRelayMode(RelayMode mode)
  {
    RelayMode = mode;
    return this;
  }
  
  public MemoryMode? MemoryMode { get; private set; }
  public LogicNode SetMemoryMode(MemoryMode mode)
  {
    MemoryMode = mode;
    return this;
  }
  
  public TimerMode? TimerMode { get; private set; }
  public LogicNode SetTimerMode(TimerMode mode)
  {
    TimerMode = mode;
    return this;
  }
  
  public TimerInterval? IntervalA { get; private set; }
  public TimerInterval? IntervalB { get; private set; }
  public LogicNode SetTimerIntervalA(TimerInterval intervalA)
  {
    IntervalA = intervalA;
    return this;
  }
  public LogicNode SetTimerIntervalB(TimerInterval intervalB)
  {
    IntervalB = intervalB;
    return this;
  }
  
  public virtual bool IsCovered { get; protected set; }
  public LogicNode SetIsCovered(bool covered)
  {
    IsCovered = covered;
    return this;
  }
  public LogicNode Covered()
  {
    IsCovered = true;
    return this;
  }
  public LogicNode UnCovered()
  {
    IsCovered = false;
    return this;
  }

  // Okay, so this breaks encapsulation best practices
  // But having these be always present makes ingestion a ton simpler
  public ISignalSource? InputA {get; set;}
  public ISignalSource? InputB {get; set;}
  public ISignalSource? ResetInput {get; set;}
  public LogicNode ConnectA(ISignalSource inputA)
  {
    if (InputA is not null)
      throw new InvalidLogicNodeConnectionException("Cannot overwrite an existing connection (A)");
    InputA = inputA;
    return this;
  }
  public LogicNode ConnectB(ISignalSource inputB)
  {
    if (InputB is not null)
      throw new InvalidLogicNodeConnectionException("Cannot overwrite an existing connection (B)");
    InputB = inputB;
    return this;
  }
  public LogicNode ConnectReset(ISignalSource resetInput)
  {
    if (ResetInput is not null)
      throw new InvalidLogicNodeConnectionException("Cannot overwrite an existing connection (Reset)");
    ResetInput = resetInput;
    return this;
  }
  
  public bool IsPinned { get; private set; }
  public LogicNode SetIsPinned(bool pinned)
  {
    IsPinned = pinned;
    return this;
  }
  public LogicNode Pinned()
  {
    IsPinned = true;
    return this;
  }
  public LogicNode UnPinned()
  {
    IsPinned = false;
    return this;
  }
  
  public bool IsSpringReturn { get; private set; }
  public LogicNode SetIsSpringReturn(bool isSpringReturn)
  {
    IsSpringReturn = isSpringReturn;
    return this;
  }
  public LogicNode Sprung()
  {
    IsSpringReturn = true;
    return this;
  }
  public LogicNode UnSprung()
  {
    IsSpringReturn = false;
    return this;
  }
  
  public Color? CustomColor { get; private set; }
  public LogicNode Color(Color color)
  {
    CustomColor = color;
    return this;
  }

  public string WhenOnHttp {get; private set; } = "http://localhost:8081/on/" + name;
  public LogicNode SetWhenOnHttp(string url)
  {
    WhenOnHttp = url;
    return this;
  }

  public string WhenOffHttp {get; private set; } = "http://localhost:8081/off/" + name;

  public LogicNode SetWhenOffHttp(string url)
  {
    WhenOffHttp = url;
    return this;
  }

  public HttpMode? HttpMode {get; private set; }
  public LogicNode SetHttpMode(HttpMode mode)
  {
    HttpMode = mode;
    return this;
  }
}