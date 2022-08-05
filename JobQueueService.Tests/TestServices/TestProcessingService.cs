using JobQueueService.Exceptions;
using JobQueueService.Models;
using JobQueueService.Models.Dto;
using JobQueueService.Models.Jobs;
using JobQueueService.Services;
using Microsoft.Extensions.Logging;
using SharpDocxTemplateModels;

namespace JobService.Tests.TestServices;

public class TestProcessingService
{
    private readonly UserJobScheduler<UniversalApplicationModel, string> _userJobScheduler;
    private string _currentUser;

    public TestProcessingService()
    {
        JobWrapper<UniversalApplicationModel, string> jobWrapper = new(ExecuteJobAsync);
        _userJobScheduler = new UserJobScheduler<UniversalApplicationModel, string>(jobWrapper, new LoggerFactory());
        _currentUser = "test";
    }

    public Guid AddJob(TemplateProcessingDto dto)
    {
        JobInput<UniversalApplicationModel> jobInput = 
            new(dto.UniversalApplicationModel, dto.Description, dto.TemplatePayloadModel);
        Guid jobId = _userJobScheduler.AddJob(jobInput, _currentUser);
        return jobId;
    }
    
    public void AddToJob(Guid jobId)
    {
        _userJobScheduler.AddToJob(jobId, GetUser());
    }
    
    public void RemoveFromJob(Guid jobId)
    {
        _userJobScheduler.RemoveFromJob(jobId, GetUser());
    }

    public string GetResult(Guid jobId)
    {
        return _userJobScheduler.GetResult(jobId, GetUser());
    }

    public JobInformation GetJobInformation(Guid jobId)
    {
        return _userJobScheduler.GetJobInformation(jobId, GetUser());
    }

    public void CancelJob(Guid jobId)
    {
        _userJobScheduler.CancelJob(jobId, GetUser());
    }

    public JobStatus GetStatus(Guid jobId)
    {
        return _userJobScheduler.GetStatus(jobId, GetUser());
    }
    
    public IEnumerable<Guid> GetJobs(JobStatus? status = null)
    {
        return _userJobScheduler.GetJobs(GetUser(),status);
    }

    public void RemoveJob(Guid jobId)
    {
        _userJobScheduler.RemoveJob(jobId, GetUser());
    }
    
    public void RemoveJobs(IEnumerable<Guid> jobIds)
    {
        _userJobScheduler.RemoveJobs(jobIds, GetUser());
    }

    public Guid CheckPayload(TemplatePayloadModel model)
    {
        IEnumerable<Guid> jobs = _userJobScheduler.GetAllJobs();
        Guid jobId = model.GetUniqueIdentifier();
        bool jobFound = jobs.Contains(jobId);
        
        if (!jobFound)
        {
            throw new JobNotFoundException(jobId);
        }

        return jobId;
    }

    public async Task<string> ExecuteJobAsync(UniversalApplicationModel data, CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), token);
        return $"Fake result for {data.TemplateName}";
    }
    
    #region Test user

    public void SetUser(string username)
    {
        _currentUser = username;
    }
    
    private string GetUser()
    {
        return _currentUser;
    }

    #endregion
}