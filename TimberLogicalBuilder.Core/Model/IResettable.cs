namespace TimberLogicalBuilder.Core.Model;

public interface IResettable
{
  ISignalSource? ResetInput { get; }
}