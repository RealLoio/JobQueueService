using JobQueueService.Models.Jobs;

namespace JobQueueService.Repositories;

public interface IJobRepository<TInput, TOutput> where TInput : class where TOutput : class
{
    /// <summary>
    /// Adds job to the pool of the jobs
    /// </summary>
    /// <param name="jobInput">Input that has data that is required to do the job and description of the job</param>
    /// <returns>Instance of the added job or an existing one</returns>
    JobModel<TInput, TOutput> AddJob(JobInput<TInput> jobInput);
    
    /// <summary>
    /// Get a list of jobs with an option to filter them by status
    /// </summary>
    /// <param name="statusFilter">Status filter for the jobs</param>
    /// <returns>Enumeration of job ids</returns>
    IEnumerable<JobModel<TInput, TOutput>> GetJobs(JobStatus? statusFilter = null);
    
    /// <summary>
    /// Get the job by id
    /// </summary>
    /// <param name="jobId">Id of the job</param>
    /// <returns>Instance to the existing job</returns>
    JobModel<TInput, TOutput> GetJob(Guid jobId);
    
    /// <summary>
    /// Remove job by id
    /// </summary>
    /// <param name="jobId">Id of the job</param>
    void RemoveJob(Guid jobId);
}
