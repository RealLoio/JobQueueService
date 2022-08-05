using JobQueueService.Exceptions;
using JobQueueService.Models.Dto;
using JobQueueService.Models.Jobs;
using JobService.Tests.TestServices;
using NUnit.Framework;

namespace JobService.Tests.JobSequenceTests;

/// <summary>
/// Adds task for user -> Cancels task -> Checks that status is cancelled -> Adds the same task ->
/// Checks that the task is the same -> Checks that status is cancelled
/// </summary>
[TestFixture]
public class JobCancellationTest
{
    private TestProcessingService _processingService;
    private TemplateProcessingDto _dto;
    private Guid _jobId;
    private const string CURRENT_USER = nameof(TestsHelper.TestUser);
    
    [OneTimeSetUp]
    public void SetUpTheTest()
    {
        _processingService = TestsHelper.CreateService();
        _dto = TestsHelper.CreateDto(nameof(SetUpTheTest), CURRENT_USER);
        _processingService.SetUser(CURRENT_USER);
        _jobId = _processingService.AddJob(_dto);
    }

    [Test]
    [Order(0)]
    public void CancelJob()
    {
        JobStatus currentStatus = _processingService.GetStatus(_jobId);
        
        Assert.DoesNotThrow(() => _processingService.CancelJob(_jobId));
        Assert.Throws<JobAccessException>(() => _processingService.GetStatus(_jobId));
    }

    [Test]
    [Order(1)]
    public void AddTheSameJob()
    {
        Guid addedJobId = Guid.Empty;
        
        Assert.DoesNotThrow(() => addedJobId = _processingService.AddJob(_dto));
        Assert.AreEqual(addedJobId, _jobId);
        Assert.AreEqual(_processingService.GetStatus(_jobId), JobStatus.Cancelled);
    }
}