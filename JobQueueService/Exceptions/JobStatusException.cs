namespace JobQueueService.Exceptions;

public class JobStatusException : JobExceptionBase
{
    public JobStatusException(Guid jobId) : base(
        $"Job with id {jobId}, didn't match status requirement",
        "The job was in the wrong status for the operation") { }
}