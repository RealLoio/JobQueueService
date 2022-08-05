using JobQueueService.Models;
using JobQueueService.Models.Jobs;
using JobQueueService.Services;
using JobService.Tests.TestJobs;
using NUnit.Framework;
using SharpDocxTemplateModels;

namespace JobService.Tests.JobSchedulerTests;

public class FailedJobsTests
{
    private UserJobScheduler<UniversalApplicationModel, string> _userJobScheduler;
    private const string TEST_USER = nameof(TestsHelper.TestUser);
    private const int JOBS_FOR_EACH_USER_COUNT = 1;
    private readonly string[] _users = {TEST_USER};
    
    [SetUp]
    public void SetUpTheTest()
    {
        JobThatFails newJob = new();
        _userJobScheduler = TestsHelper.CreateScheduler(newJob);
        
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
    public void FailedStatus(string username)
    {
        Guid jobId = _userJobScheduler.GetJobs(username).FirstOrDefault();

        while (_userJobScheduler.GetStatus(jobId, username) != JobStatus.Failed)
        {
            Task.Delay(TimeSpan.FromSeconds(1));
        }
        
        Assert.AreEqual(JobStatus.Failed, _userJobScheduler.GetStatus(jobId, username));
    }
}