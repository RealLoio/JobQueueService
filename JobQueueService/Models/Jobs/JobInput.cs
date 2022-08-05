namespace JobQueueService.Models.Jobs;

public class JobInput<TInput> where TInput : class
{
    public IUniquePayload Payload { get; }
    public TInput Data { get; }
    public string Description { get; }
    public JobInput(TInput data, string description, IUniquePayload payload)
    {
        this.Data = data;
        this.Description = description;
        this.Payload = payload;
    }
}