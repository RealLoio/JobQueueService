using JobQueueService.Exceptions;
using JobQueueService.Models.Jobs;
using Microsoft.Extensions.Logging;

namespace JobQueueService.Repositories;

/// <inheritdoc/>
public sealed class JobRepository<TInput, TOutput> :
    IJobRepository<TInput, TOutput>
    where TInput : class
    where TOutput : class
{
    private readonly ILogger<JobRepository<TInput, TOutput>> _logger;
    private readonly Dictionary<Guid, JobModel<TInput, TOutput>> _jobs = new();

    public JobRepository(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<JobRepository<TInput, TOutput>>();
    }
    
    public JobModel<TInput, TOutput> AddJob(JobInput<TInput> jobInput) 
    {
        Guid jobId = jobInput.Payload.GetUniqueIdentifier();

        if (!_jobs.TryGetValue(jobId, out JobModel<TInput, TOutput>? job))
        {
            job = new JobModel<TInput, TOutput>(jobId, jobInput.Data, jobInput.Description);
            _jobs.TryAdd(jobId, job);
        }
        
        return job;
    }

    public IEnumerable<JobModel<TInput, TOutput>> GetJobs(JobStatus? statusFilter = null)
    {
        return _jobs.Values;
    }
    
    ///<inheritdoc/>
    /// <exception cref="JobNotFoundException">Job wasn't found</exception>
    public JobModel<TInput, TOutput> GetJob(Guid jobId)
    {
        if (!_jobs.TryGetValue(jobId, out JobModel<TInput, TOutput>? job))
        {
            _logger.LogDebug("Job {JobId} wasn't found", jobId);
            throw new JobNotFoundException(jobId);
        }

        return job;
    }

    ///<inheritdoc/>
    /// <exception cref="JobNotFoundException">Job wasn't found</exception>
    public void RemoveJob(Guid jobId)
    {
        if (!_jobs.TryGetValue(jobId, out _))
        {
            _logger.LogDebug("Wasn't able to remove the job because job with id {JobId} wasn't found", jobId);
            throw new JobNotFoundException(jobId);
        }
        
        _jobs.Remove(jobId);
    }
}