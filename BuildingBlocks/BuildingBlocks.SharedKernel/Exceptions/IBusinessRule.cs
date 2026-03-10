namespace BuildingBlocks.SharedKernel.Exceptions;

public interface IBusinessRule
{
    bool IsBroken();
    string Message { get; }
}
