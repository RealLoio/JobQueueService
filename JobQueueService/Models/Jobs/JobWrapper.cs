namespace JobQueueService.Models.Jobs;

public class JobWrapper<TInput, TOutput> : IAsyncJob<TInput, TOutput>
    where TInput : class
    where TOutput : class
{
    private readonly Func<TInput, CancellationToken, Task<TOutput>> _job;
    public JobWrapper(Func<TInput, CancellationToken, Task<TOutput>> job)
    {
        _job = job;
    }
    
    public async Task<TOutput> ExecuteJobAsync(TInput data, CancellationToken cancellationToken)
    {
        return await _job.Invoke(data, cancellationToken);
    }
}