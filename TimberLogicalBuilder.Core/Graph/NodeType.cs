namespace TimberLogicalBuilder.Core.Graph;

// Now that we have both regular levers and http levers, can't just rely on the subtype enum to differentiate the various types of node. :(
public enum NodeType
{
  Empty,
  Lever,
  Relay,
  Memory,
  Timer,
  Indicator,
  HttpLever,
  HttpAdapter
}