namespace AlGreenMES.BuildingBlocks.Common.Exceptions;

public record ValidationError(string Property, string Message);

public class ValidationException : Exception
{
    public IReadOnlyList<ValidationError> Errors { get; }

    public ValidationException(IReadOnlyList<ValidationError> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public ValidationException(string property, string message)
        : this(new List<ValidationError> { new(property, message) })
    {
    }
}
