namespace JobQueueService.Models.Jobs;

public interface IAsyncJob<in TInput, TOutput> 
    where TInput : class
    where TOutput : class
{
    Task<TOutput> ExecuteJobAsync(TInput arg, CancellationToken token);
}