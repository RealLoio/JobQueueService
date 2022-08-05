using JobQueueService.Exceptions;
using JobQueueService.Models;
using JobQueueService.Models.Jobs;
using JobQueueService.Services;
using JobService.Tests.TestJobs;
using NUnit.Framework;
using SharpDocxTemplateModels;

namespace JobService.Tests.JobSchedulerTests;

public class NullResultTest
{
    private UserJobScheduler<UniversalApplicationModel, string> _userJobScheduler;
    private const string TEST_USER = nameof(TestsHelper.TestUser);
    private const string BASIC_USER = nameof(TestsHelper.BasicUser);
    private const int JOBS_FOR_EACH_USER_COUNT = 3;
    private readonly string[] _users = {TEST_USER, BASIC_USER};
    
    [SetUp]
    public void SetUpTheTest()
    {
        JobWithNoResult noResultJob = new();
        _userJobScheduler = TestsHelper.CreateScheduler(noResultJob);
        
        foreach (string user in _users)
        {
            for (int i = 0; i < JOBS_FOR_EACH_USER_COUNT; i++)
            {
                TemplatePayloadModel templatePayloadModel = TestsHelper.GetPayload(nameof(SetUpTheTest), user, i);
                _userJobScheduler.AddJob(TestsHelper.JobInput(templatePayloadModel), user);
            }
        }
    }

    [Test]
    [TestCase(TEST_USER)]
    public void GetNullResults(string username)
    {
        IEnumerable<Guid> jobIds = _userJobScheduler.GetJobs(username);
        Guid jobId = jobIds.FirstOrDefault();
        
        while (_userJobScheduler.GetStatus(jobId, username) != JobStatus.Finished)
        {
            Task.Delay(100);
        }

        Assert.Throws<JobNoResultException>(() => _userJobScheduler.GetResult(jobId, username));
    }
}