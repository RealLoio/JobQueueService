using System.Threading.Tasks.Dataflow;
using JobQueueService.Exceptions;
using JobQueueService.Models.Jobs;
using JobQueueService.Repositories;
using Microsoft.Extensions.Logging;

namespace JobQueueService.Services;

public abstract class JobSchedulerBase<TInput, TOutput>
    where TInput : class
    where TOutput : class
{
    private readonly IAsyncJob<TInput, TOutput> _job;
    private readonly ActionBlock<JobModel<TInput, TOutput>> _jobExecutionBlock;
    protected readonly JobRepository<TInput, TOutput> JobRepository;
    protected readonly ILogger<JobSchedulerBase<TInput, TOutput>> Logger;

    protected JobSchedulerBase(IAsyncJob<TInput, TOutput> jobAsync, ILoggerFactory loggerFactory)
    {
        _job = jobAsync;
        ExecutionDataflowBlockOptions options = new() {MaxDegreeOfParallelism = 1};
        _jobExecutionBlock = new ActionBlock<JobModel<TInput, TOutput>>(ExecuteJob, options);
        Logger = loggerFactory.CreateLogger<JobSchedulerBase<TInput, TOutput>>();
        JobRepository = new JobRepository<TInput, TOutput>(loggerFactory);
    }

    /// <summary>
    /// Adds job to the pool of the jobs
    /// </summary>
    /// <param name="jobInput">Input that has data that is required to do the job and description of the job</param>
    /// <returns>Id of the added job, or id of already existing one</returns>
    protected Guid AddJob(JobInput<TInput> jobInput)
    {
        JobModel<TInput, TOutput> job = JobRepository.AddJob(jobInput);
        
        if (!job.HasStarted())
        {
            _jobExecutionBlock.Post(job);
            job.SetStatus(JobStatus.InQueue);
        }
        
        return job.JobId;
    }

    /// <summary>
    /// Get the information about the job. The information includes description, job id, time started, time finished and time posted
    /// </summary>
    /// <param name="jobId">Id of the job</param>
    /// <returns>Returns the model with information about the job</returns>
    protected JobInformation GetJobInformation(Guid jobId)
    {
        JobModel<TInput, TOutput> job = GetJob(jobId);
        return new JobInformation(job.JobId, job.Status, job.JobDetails);
    }

    /// <summary>
    /// Get a list of jobs with an option to filter them by status
    /// </summary>
    /// <param name="statusFilter">Status filter for the jobs</param>
    /// <returns>Enumeration of job ids</returns>
    protected IEnumerable<Guid> GetJobs(JobStatus? statusFilter = null)
    {
        IEnumerable<JobModel<TInput, TOutput>> jobs = JobRepository.GetJobs();
        
        if (statusFilter is null)
        {
            return jobs.Select(o=> o.JobId);
        }
        
        return jobs.Where(o => o.Status == statusFilter).Select(o => o.JobId);
    }

    /// <summary>
    /// Get status of the job
    /// </summary>
    /// <param name="jobId">Id of the job</param>
    /// <returns>Job status</returns>
    protected JobStatus GetStatus(Guid jobId)
    {
        JobModel<TInput, TOutput> job = JobRepository.GetJob(jobId);
        return job.Status;
    }

    /// <summary>
    /// Remove job by id
    /// </summary>
    /// <param name="jobId">Id of the job</param>
    protected void RemoveJob(Guid jobId)
    {
        JobModel<TInput, TOutput> job = JobRepository.GetJob(jobId);
        if (!job.IsFinished())
        {
            CancelJob(job);
        }
        
        JobRepository.RemoveJob(job.JobId);
    }

    /// <summary>
    /// Remove a group of jobs by ids
    /// </summary>
    /// <param name="jobIds">Jobs ids</param>
    protected void RemoveJobs(IEnumerable<Guid> jobIds)
    {
        foreach (Guid jobId in jobIds)
        {
            RemoveJob(jobId);
        }
    }
    
    
    /// <summary>
    /// Cancel job of the user by id
    /// </summary>
    /// <param name="jobId">Id of the job</param>
    /// <exception cref="JobNotFoundException">Job wasn't found</exception>
    protected void CancelJob(Guid jobId)
    {
        JobModel<TInput, TOutput> job =  JobRepository.GetJob(jobId);

        if (!job.IsInWork())
        {
            Logger.LogDebug("Job wasn't in work, job id: {JobId}", jobId);
            throw new JobStatusException(jobId);
        }

        CancelJob(job);
    }

    /// <summary>
    /// Get the result of the job by it's id
    /// </summary>
    /// <param name="jobId">Job id</param>
    /// <returns>Result of the job</returns>
    /// <exception cref="JobStatusException">Job isn't finished</exception>
    /// <exception cref="JobNoResultException">The result of the job is NULL></exception>
    protected TOutput GetResult(Guid jobId)
    {
        JobModel<TInput, TOutput> job =  JobRepository.GetJob(jobId);

        if (!job.IsFinished())
        {
            Logger.LogDebug("Job wasn't finished, job id: {JobId}", jobId);
            throw new JobStatusException(jobId);
        }
        
        return job.Result ?? throw new JobNoResultException(job.JobId);
    }
    
    /// <summary>
    /// Triggers the cancellation token of the passed job
    /// </summary>
    /// <param name="job">Job to cancel</param>
    protected void CancelJob(JobModel<TInput, TOutput> job)
    {
        job.SetStatus(JobStatus.Cancelled);
        Logger.LogInformation("Job cancellation was requested for {JobId}", job.JobId);
        job.CancellationTokenSource.Cancel();
    }

    /// <summary>
    /// Get a job by id
    /// </summary>
    /// <param name="jobId">Id of the job</param>
    /// <returns><see cref="JobModel{TInput,TOutput}"/></returns>
    private JobModel<TInput, TOutput> GetJob(Guid jobId)
    {
        return JobRepository.GetJob(jobId);
    }

    /// <summary>
    /// Action that contains the method executed in the action block and check if the token is cancelled
    /// </summary>
    /// <param name="jobModel">Job to execute</param>
    /// <returns></returns>
    public async Task ExecuteJob(JobModel<TInput, TOutput> jobModel)
    {
        try
        {
            if (jobModel.CancellationTokenSource.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }
            
            jobModel.StartJob();
            jobModel.Result = await _job.ExecuteJobAsync(jobModel.Data, jobModel.CancellationTokenSource.Token);
            jobModel.EndJob(JobStatus.Finished);
        }
        catch (OperationCanceledException)
        {
            Logger.LogInformation("Job {JobId} was cancelled", jobModel.JobId);
        }
        catch (Exception ex)
        {
            jobModel.EndJob(JobStatus.Failed);
            Logger.LogError(ex, "Error while executing job. Job id: {JobId}", jobModel.JobId);
        }
    }
}