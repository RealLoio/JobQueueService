using System.Diagnostics.CodeAnalysis;
using JobQueueService.Exceptions;
using JobQueueService.Models.Jobs;
using JobQueueService.Repositories;
using Microsoft.Extensions.Logging;

namespace JobQueueService.Services;

public sealed class UserJobScheduler<TInput, TOutput> : JobSchedulerBase<TInput, TOutput>  
    where TInput : class
    where TOutput : class
{
    private readonly UserRepository _userRepository;

    public UserJobScheduler(IAsyncJob<TInput, TOutput> jobAsync, ILoggerFactory loggerFactory) : base(jobAsync, loggerFactory)
    {
        _userRepository = new UserRepository();
    }
    
    /// <param name="username">User starting the job</param>
    /// <inheritdoc cref="JobSchedulerBase{TInput,TOutput}.AddJob{TModel}"/>
    [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
    public Guid AddJob(JobInput<TInput> jobInput, string username) 
    {
        Guid jobId = base.AddJob(jobInput);
        AddToJob(jobId, username);
        return jobId;
    }

    /// <inheritdoc cref="JobSchedulerBase{TInput,TOutput}.GetJobInformation"/>
    /// <param name="username">User who is subscribed to the job</param>
    /// <exception cref="Exception">Was unable to verify the user</exception>
    [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
    public JobInformation GetJobInformation(Guid jobId, string username)
    {
        if (!_userRepository.HasJob(jobId, username))
        {
            throw new JobAccessException(username, jobId);
        }

        return base.GetJobInformation(jobId);
    }

    /// <summary>
    /// Get a list of jobs the user is subscribed to.
    /// </summary>
    /// <param name="user">The jobs will be checked if this user is subscribed to them.</param>
    /// <param name="statusFilter">Option to filter jobs by status</param>
    /// <returns>Enumeration of job ids</returns>
    public IEnumerable<Guid> GetJobs(string user, JobStatus? statusFilter = null)
    {
        IEnumerable<Guid> jobs = GetJobs(statusFilter);
        
        foreach (Guid jobId in jobs)
        {
            if (_userRepository.HasJob(jobId, user))
            {
                yield return jobId;
            }
        }
    }

    /// <inheritdoc cref="JobSchedulerBase{TInput,TOutput}.GetJobs"/>
    public IEnumerable<Guid> GetAllJobs(JobStatus? statusFilter = null)
    {
        return base.GetJobs(statusFilter);
    }

    /// <summary>
    /// Get status of the job
    /// </summary>
    /// <param name="jobId">Id of the job</param>
    /// <param name="username">User who is subscribed to the job</param>
    /// <returns>Job status</returns>
    public JobStatus GetStatus(Guid jobId, string username)
    {
        JobModel<TInput, TOutput> job =  JobRepository.GetJob(jobId);
        
        if (!_userRepository.HasJob(jobId, username))
        {
            Logger.LogDebug("User {Username} wasn't subscribed to the job {JobId}", username, jobId);
            throw new JobAccessException(username, jobId);
        }
        
        return job.Status;
    }

    /// <inheritdoc cref="JobSchedulerBase{TInput,TOutput}.RemoveJob"/>
    /// <param name="username">User removing the job</param>
    [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
    public void RemoveJob(Guid jobId, string username)
    {
        if (!_userRepository.HasJob(jobId, username))
        {
            throw new JobAccessException(username, jobId);
        }

        RemoveFromJob(jobId, username);
        
        if (_userRepository.AnyUsers(jobId))
        {
            return;
        }
        
        base.RemoveJob(jobId);
    }

    /// <summary>
    /// Remove a group of job by ids
    /// </summary>
    /// <param name="jobIds">Jobs ids</param>
    /// <param name="username">User who is removing jobs</param>
    public void RemoveJobs(IEnumerable<Guid> jobIds, string username)
    {
        foreach (Guid jobId in jobIds)
        {
            RemoveJob(jobId, username);
        }
    }

    public void AddToJob(Guid jobId, string username)
    {
        _userRepository.AddToJob(jobId, username);
    }
    
    public void RemoveFromJob(Guid jobId, string username)
    {
        _userRepository.RemoveFromJob(jobId, username);
    }

    /// <summary>
    /// Cancel a job of the user by id
    /// </summary>
    /// <param name="jobId">Id of the job</param>
    /// <param name="username">User cancelling the job</param>
    /// <exception cref="JobNotFoundException">Job wasn't found</exception>
    public void CancelJob(Guid jobId, string username)
    {
        JobModel<TInput, TOutput> job =  JobRepository.GetJob(jobId);
        
        if (!_userRepository.HasJob(jobId, username))
        {
            throw new JobAccessException(username, jobId);
        }
        
        if (!job.IsInWork())
        {
            throw new JobStatusException(jobId);
        }

        CancelJob(job, username);
    }

    /// <summary>
    /// Get the result of the job by it's id
    /// </summary>
    /// <param name="jobId">Id of the job</param>
    /// <param name="username">User accessing the job</param>
    /// <returns>Result of the job</returns>
    /// <exception cref="JobAccessException">User isn't subscribed to the job</exception>
    /// <exception cref="JobStatusException">Job isn't finished</exception>
    /// <exception cref="JobNoResultException">The result of the job is NULL</exception>
    public TOutput GetResult(Guid jobId, string username)
    {
        JobModel<TInput, TOutput> job =  JobRepository.GetJob(jobId);
        
        if (!_userRepository.HasJob(jobId, username))
        {
            Logger.LogDebug("User {Username} wasn't subscribed to the job {JobId}", username, jobId);
            throw new JobAccessException(username, jobId);
        }

        if (!job.IsFinished())
        {
            throw new JobStatusException(jobId);
        }

        TOutput result = job.Result ?? throw new JobNoResultException(job.JobId);
        RemoveFromJob(job.JobId, username);
        return result;
    }

    /// <summary>
    /// Cancel the passed job for the passed user
    /// </summary>
    /// <param name="job">Job to cancel</param>
    /// <param name="username">User who is cancelling the job</param>
    private void CancelJob(JobModel<TInput, TOutput> job, string username)
    {
        RemoveFromJob(job.JobId, username);
        if (_userRepository.AnyUsers(job.JobId))
        {
            return;
        }
        
        CancelJob(job);
    }
}