namespace JobQueueService.Exceptions;

public abstract class JobExceptionBase : Exception
{
    private const string DEFAULT_MESSAGE = "An error occured during the execution";
    public readonly string DisplayMessage;

    protected JobExceptionBase(string message, string displayMessage = DEFAULT_MESSAGE) : base(message)
    {
        DisplayMessage = displayMessage;
    }
}