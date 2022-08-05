namespace JobQueueService.Repositories;

public interface IUsersRepository
{
    /// <summary>
    /// Subscribe user to a job
    /// </summary>
    /// <param name="username">User</param>
    /// <param name="jobId">Job to subscribe to</param>
    void AddToJob(Guid jobId, string username);
    
    /// <summary>
    /// Checks if user has a job
    /// </summary>
    /// <param name="username">User</param>
    /// <param name="jobId">Id of the job</param>
    /// <returns>True if user is subscribed to the job</returns>
    bool HasJob(Guid jobId, string username);
    
    /// <summary>
    /// Unsubscribes user from the job
    /// </summary>
    /// <param name="username">User</param>
    /// <param name="jobId">Id of the job to unsubscribe from</param>
    void RemoveFromJob(Guid jobId, string username);
    
    /// <summary>
    /// Checks if any users are subscribed to the job
    /// </summary>
    /// <param name="jobId">Id of the job</param>
    /// <returns>True if at least one user is subscribed to the job</returns>
    bool AnyUsers(Guid jobId);
}