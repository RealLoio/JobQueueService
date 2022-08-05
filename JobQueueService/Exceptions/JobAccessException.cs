namespace JobQueueService.Exceptions;

public sealed class JobAccessException : JobExceptionBase
{
    public JobAccessException(string username, Guid jobId) : base(
        $"{username} doesn't have access for job {jobId}", 
        $"User {username} didn't have access to the job") { }
}