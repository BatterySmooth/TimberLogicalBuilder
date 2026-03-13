namespace TimberLogicalBuilder.Core.Builder;

public struct LogicBuilderSettings()
{
  public bool reuseExisting {get; private set;} = true;
  public bool preserveExistingConnections {get; private set;} = false;

  public bool preserveExistingNullConnections {get; private set;} = false;

  public LogicBuilderSettings ignoreExisting()
  {
    reuseExisting = false;
    return this;
  }

  public LogicBuilderSettings PreserveExistingConnections()
  {
    preserveExistingConnections = true;
    return this;
  }

  public LogicBuilderSettings PreserveExistingNulls()
  {
    preserveExistingNullConnections = true;
    return this;
  }
}