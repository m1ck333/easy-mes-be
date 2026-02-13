namespace AlGreenMES.BuildingBlocks.Common.Exceptions;

public class ForbiddenException : DomainException
{
    public ForbiddenException(string message)
        : base("FORBIDDEN", message)
    {
    }

    public ForbiddenException()
        : base("FORBIDDEN", "You do not have permission to perform this action.")
    {
    }
}
