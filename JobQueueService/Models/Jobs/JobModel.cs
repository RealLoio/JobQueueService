namespace JobQueueService.Models.Jobs;

public class JobModel<TInput, TOutput>
    where TInput : class
    where TOutput : class
{
    public Guid JobId { get; }
    public TInput Data { get; }
    public JobDetails JobDetails { get; }
    public JobStatus Status { get; private set; }
    public CancellationTokenSource CancellationTokenSource { get; }
    public TOutput? Result { get; set; }

    public JobModel(Guid jobId, TInput data, string description)
    {
        this.JobId = jobId;
        this.Data = data;
        this.CancellationTokenSource = new CancellationTokenSource();
        this.JobDetails = new JobDetails(description);
    }

    public void StartJob()
    {
        SetStatus(JobStatus.InProcess);
        this.JobDetails.UpdateTimeStarted();
    }

    public void EndJob(JobStatus status)
    {
        SetStatus(status);
        this.JobDetails.UpdateTimeFinished();
    }

    public bool IsFinished()
    {
        return this.Status is JobStatus.Cancelled or JobStatus.Failed or JobStatus.Finished;
    }

    public bool HasStarted()
    {
        return this.Status is not JobStatus.NotStarted;
    }

    public bool IsInWork()
    {
        return this.Status is JobStatus.InQueue or JobStatus.InProcess;
    }

    public void SetStatus(JobStatus newStatus)
    {
        this.Status = newStatus;
    }
}