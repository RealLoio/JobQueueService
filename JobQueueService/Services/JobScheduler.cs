using JobQueueService.Exceptions;
using JobQueueService.Models.Jobs;
using Microsoft.Extensions.Logging;

namespace JobQueueService.Services;

/// <inheritdoc cref="JobSchedulerBase{TInput,TOutput}"/>
public class JobScheduler<TInput, TOutput> : JobSchedulerBase<TInput, TOutput>  
    where TInput : class
    where TOutput : class
{
    public JobScheduler(IAsyncJob<TInput, TOutput> jobAsync, ILoggerFactory loggerFactory) : base(jobAsync, loggerFactory)
    {
    }

    /// <inheritdoc cref="JobSchedulerBase{TInput,TOutput}.AddJob"/>
    new public Guid AddJob(JobInput<TInput> jobInput)
    {
        return base.AddJob(jobInput);
    }
    
    /// <inheritdoc cref="JobSchedulerBase{TInput,TOutput}.GetJobInformation"/>
    new protected JobInformation GetJobInformation(Guid jobId) => base.GetJobInformation(jobId);

    /// <inheritdoc cref="JobSchedulerBase{TInput,TOutput}.GetJobs"/>
    new protected IEnumerable<Guid> GetJobs(JobStatus? statusFilter = null) => base.GetJobs(statusFilter);

    /// <inheritdoc cref="JobSchedulerBase{TInput,TOutput}.GetStatus"/>
    new protected JobStatus GetStatus(Guid jobId) => base.GetStatus(jobId);
    

    /// <inheritdoc cref="JobSchedulerBase{TInput,TOutput}.RemoveJob"/>
    new protected void RemoveJob(Guid jobId) => base.RemoveJob(jobId);

    /// <inheritdoc cref="JobSchedulerBase{TInput,TOutput}.RemoveJobs"/>
    new protected void RemoveJobs(IEnumerable<Guid> jobIds) => base.RemoveJobs(jobIds);
    
    /// <summary>
    /// Cancel job of the user by id
    /// </summary>
    /// <param name="jobId">Id of the job</param>
    /// <exception cref="JobNotFoundException">Job wasn't found</exception>
    new protected void CancelJob(Guid jobId)
    {
        JobModel<TInput, TOutput> job =  JobRepository.GetJob(jobId);

        if (!job.IsInWork())
        {
            Logger.LogDebug("Job wasn't in work, job id: {JobId}", jobId);
            throw new JobStatusException(jobId);
        }

        CancelJob(job);
    }

    /// <inheritdoc cref="JobSchedulerBase{TInput,TOutput}.GetResult"/>
    new protected TOutput GetResult(Guid jobId) => base.GetResult(jobId);
}