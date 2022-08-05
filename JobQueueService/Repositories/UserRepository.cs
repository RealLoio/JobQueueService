using JobQueueService.Exceptions;

namespace JobQueueService.Repositories;

/// <inheritdoc/>
public class UserRepository : IUsersRepository
{
    private readonly Dictionary<Guid, List<string>> _jobUsers = new();
    
    public void AddToJob(Guid taskId, string username)
    {
        if (!_jobUsers.TryGetValue(taskId, out List<string>? users))
        {
            _jobUsers.TryAdd(taskId, new List<string>() {username});
            return;
        }

        if (users.Contains(username))
        {
            return;
        }
        
        users.Add(username);
    }

    public bool HasJob(Guid taskId, string username)
    {
        if (!_jobUsers.ContainsKey(taskId))
        {
            throw new JobNotFoundException(taskId);
        }
        
        return _jobUsers.TryGetValue(taskId, out List<string>? users) && users.Contains(username);
    }

    public void RemoveFromJob(Guid taskId, string username)
    {
        IEnumerable<Guid> tasks = GetTasks(username);
        
        if (!tasks.Contains(taskId))
        {
            return;
        }

        if (_jobUsers.TryGetValue(taskId, out List<string>? users))
        {
            users.Remove(username);
        }
    }

    public bool AnyUsers(Guid taskId)
    {
        return _jobUsers.TryGetValue(taskId, out List<string>? users) && users.Any();
    }

    /// <summary>
    /// Get tasks of the user
    /// </summary>
    /// <param name="username">User</param>
    /// <returns>Enumeration that contains the ids of the tasks</returns>
    private IEnumerable<Guid> GetTasks(string username)
    {
        foreach ((Guid taskId, List<string> users) in _jobUsers)
        {
            if (users.Contains(username))
            {
                yield return taskId;
            }
        }
    }
}