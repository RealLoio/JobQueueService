namespace JobQueueService.Models.Jobs;

public enum JobStatus
{
    NotStarted,
    InQueue,
    InProcess,
    Finished,
    Cancelled,
    Failed
}