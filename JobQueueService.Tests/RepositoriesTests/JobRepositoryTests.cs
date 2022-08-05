using JobQueueService.Exceptions;
using JobQueueService.Models;
using JobQueueService.Repositories;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SharpDocxTemplateModels;

namespace JobService.Tests.RepositoriesTests;

public class JobRepositoryTests
{
    private IJobRepository<UniversalApplicationModel, string> _jobRepository;
    private const int TOTAL_JOBS_COUNT = 3;

    [SetUp]
    public void SetUpTheTest()
    {
        LoggerFactory loggerFactory = new();
        _jobRepository = new JobRepository<UniversalApplicationModel, string>(loggerFactory);
        
        for (int i = 0; i < TOTAL_JOBS_COUNT; i++)
        {
            TemplatePayloadModel templatePayloadModel =
                TestsHelper.GetPayload(nameof(SetUpTheTest), nameof(SetUpTheTest), i);
            _jobRepository.AddJob(TestsHelper.JobInput(templatePayloadModel));
        }
    }
    
    [Test]
    public void GetAllJobsTest()
    {
        int allJobsCount = _jobRepository.GetJobs().Count();
        
        Assert.AreEqual(TOTAL_JOBS_COUNT, allJobsCount);
    }

    [Test]
    public void GetJobTest()
    {
        Guid nonExistentId = Guid.NewGuid();
        Guid existingJob = _jobRepository.GetJobs().Select(o => o.JobId).FirstOrDefault();
        
        Assert.Throws<JobNotFoundException>(() => _jobRepository.GetJob(nonExistentId));
        Assert.DoesNotThrow(() => _jobRepository.GetJob(existingJob));
    }
    
    [Test]
    public void JobRemovalTest()
    {
        Guid nonExistentJobId = Guid.NewGuid();
        Guid existingJob = _jobRepository.GetJobs().Select(o => o.JobId).FirstOrDefault();
        
        Assert.Throws<JobNotFoundException>(() => _jobRepository.RemoveJob(nonExistentJobId));
        Assert.DoesNotThrow(() => _jobRepository.RemoveJob(existingJob));
    }
}