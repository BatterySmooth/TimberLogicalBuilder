namespace TimberLogicalBuilder.Core.Exceptions;

[Serializable]
public class InvalidLogicNodeConnectionException : Exception
{
  public InvalidLogicNodeConnectionException() { }

  public InvalidLogicNodeConnectionException(string message)
    : base(message) { }

  public InvalidLogicNodeConnectionException(string message, Exception innerException)
    : base(message, innerException) { }
}