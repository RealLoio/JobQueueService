namespace JobQueueService.Models.Jobs;

public class JobInformation
{
    public Guid JobId { get; }
    public string Description { get; }
    public JobStatus Status { get; }
    public DateTime PostedAt { get; }
    public DateTime? StartedAt { get; }
    public DateTime? FinishedAt { get; }

    public JobInformation(Guid jobId, JobStatus status, JobDetails details)
    {
        this.JobId = jobId;
        this.Status = status;
        this.Description = details.Description;
        this.PostedAt = details.PostedAt;
        this.StartedAt = details.StartedAt;
        this.FinishedAt = details.FinishedAt;
    }
}