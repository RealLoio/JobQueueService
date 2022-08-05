namespace JobQueueService.Models;

public interface IUniquePayload
{
    Guid GetUniqueIdentifier();
}