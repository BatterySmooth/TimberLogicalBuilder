using System.Runtime.Serialization;

namespace TimberLogicalBuilder.Core.Exceptions;

[Serializable]
public class InvalidLogicNodeConnectionException : Exception
{
  public InvalidLogicNodeConnectionException()
  {
  }

  public InvalidLogicNodeConnectionException(string message)
    : base(message)
  {
  }

  public InvalidLogicNodeConnectionException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  [Obsolete("Obsolete")]
  protected InvalidLogicNodeConnectionException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}