namespace JobQueueService.Models.Jobs;

public class JobDetails
{
    public string Description { get; }
    public DateTime PostedAt { get; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? FinishedAt { get; private set; }

    public JobDetails(string description)
    {
        this.Description = description;
        this.PostedAt = DateTime.UtcNow;
    }

    public void UpdateTimeStarted()
    {
        this.StartedAt = DateTime.UtcNow;
    }

    public void UpdateTimeFinished()
    {
        this.FinishedAt = DateTime.UtcNow;
    }
}