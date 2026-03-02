namespace Templater.Core.Exceptions;

public class TemplaterInvalidOperationException : TemplaterException
{
    public TemplaterInvalidOperationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public TemplaterInvalidOperationException(string message) : base(message)
    {
    }
}
