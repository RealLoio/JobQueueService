namespace JobQueueService.Exceptions;

public sealed class JobNotFoundException : JobExceptionBase
{
    public JobNotFoundException(Guid jobId) : base(
        $"No job with id {jobId}",
        "The job wasn't found") { }
}