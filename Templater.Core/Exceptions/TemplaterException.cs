namespace Templater.Core.Exceptions;

public abstract class TemplaterException : ApplicationException
{
    protected TemplaterException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected TemplaterException(string message) : base(message)
    {
    }
}
