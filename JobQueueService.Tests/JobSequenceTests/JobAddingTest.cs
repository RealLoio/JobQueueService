using JobQueueService.Exceptions;
using JobQueueService.Models.Dto;
using JobQueueService.Models.Jobs;
using JobService.Tests.TestServices;
using NUnit.Framework;

namespace JobService.Tests.JobSequenceTests;

public class JobAddingTest
{
    private TestProcessingService _processingService;
    private TemplateProcessingDto _dto;
    private Guid _jobId;
    private const string CURRENT_USER = nameof(TestsHelper.TestUser);
    
    [OneTimeSetUp]
    public void SetUpTheJob()
    {
        _processingService = TestsHelper.CreateService();
        _dto = TestsHelper.CreateDto(nameof(SetUpTheJob), CURRENT_USER);
        _processingService.SetUser(CURRENT_USER);
    }

    [Test]
    [Order(0)]
    public void AddJob()
    {
        Assert.Throws<JobNotFoundException>(() => _processingService.CheckPayload(_dto.TemplatePayloadModel));
        Assert.DoesNotThrow(() => _jobId = _processingService.AddJob(_dto));
    }

    [Test]
    [Order(1)]
    public void CancelJob()
    {
        Assert.DoesNotThrow(() => _processingService.CancelJob(_jobId));
    }

    [Test]
    [Order(2)]
    public void SubscribeToJob()
    {
        Assert.DoesNotThrow(() => _processingService.CheckPayload(_dto.TemplatePayloadModel));
        Assert.DoesNotThrow(() =>  _processingService.AddToJob(_jobId));
        Assert.AreEqual(JobStatus.Cancelled, _processingService.GetStatus(_jobId));
    }
}