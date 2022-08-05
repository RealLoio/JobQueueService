namespace JobQueueService.Exceptions;

public sealed class JobNoResultException : JobExceptionBase
{
    public JobNoResultException(Guid jobId) : base(
        $"No result yet in jod {jobId}",
        "The job doesn't have result yet") { }
}