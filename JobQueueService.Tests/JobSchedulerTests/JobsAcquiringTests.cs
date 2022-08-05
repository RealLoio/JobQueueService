using JobQueueService.Models;
using JobQueueService.Services;
using JobService.Tests.TestJobs;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SharpDocxTemplateModels;

namespace JobService.Tests.JobSchedulerTests;

public class JobsAcquiringTests
{
    private UserJobScheduler<UniversalApplicationModel, string> _userJobScheduler;
    private const string TEST_USER = nameof(TestsHelper.TestUser);
    private const string BASIC_USER = nameof(TestsHelper.BasicUser);
    private const int JOBS_FOR_EACH_USER_COUNT = 3;
    private readonly string[] _users = {TEST_USER, BASIC_USER};
    private int _totalJobsCount;

    [SetUp]
    public void SetUpTheTest()
    {
        _totalJobsCount = JOBS_FOR_EACH_USER_COUNT * _users.Length;
        LoggerFactory loggerFactory = new();
        TestJob newJob = new(loggerFactory);
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
    public void CheckAllJobsCount()
    {
        int totalNumber = _users.Sum(user => _userJobScheduler.GetJobs(user).Count());
        Assert.AreEqual(_totalJobsCount, totalNumber);
    }

    [Test]
    public void CheckJobsCountForEachUser()
    {
        foreach (string user in _users)
        {
            int jobsCount = _userJobScheduler.GetJobs(user).Count();
            Assert.AreEqual(JOBS_FOR_EACH_USER_COUNT, jobsCount);
        }
    }
}