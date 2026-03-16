namespace TimberLogicalBuilder.Components.Exceptions;

[Serializable]
public class DynamicComponentOverflowException : Exception
{
  public DynamicComponentOverflowException() { }

  public DynamicComponentOverflowException(string message)
    : base(message) { }

  public DynamicComponentOverflowException(string message, Exception innerException)
    : base(message, innerException) { }
}