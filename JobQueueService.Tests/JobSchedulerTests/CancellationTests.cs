using JobQueueService.Exceptions;
using JobQueueService.Models;
using JobQueueService.Models.Jobs;
using JobQueueService.Services;
using JobService.Tests.TestJobs;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SharpDocxTemplateModels;

namespace JobService.Tests.JobSchedulerTests;

public class CancellationTests
{
    private UserJobScheduler<UniversalApplicationModel, string> _userJobScheduler;
    private const string TEST_USER = nameof(TestsHelper.TestUser);
    private const string BASIC_USER = nameof(TestsHelper.BasicUser);
    private const int JOBS_FOR_EACH_USER_COUNT = 3;
    private readonly string[] _users = {TEST_USER, BASIC_USER};
    
    [SetUp]
    public void SetUpTheTest()
    {
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
    [TestCase(TEST_USER)]
    public void CancelJob(string username)
    {
        Guid jobId = _userJobScheduler.GetJobs(username).FirstOrDefault();
        
        Assert.DoesNotThrow(() => _userJobScheduler.CancelJob(jobId, username));
        Assert.Throws<JobAccessException>(() => _userJobScheduler.CancelJob(jobId, username));

        Assert.DoesNotThrow(() => _userJobScheduler.AddToJob(jobId, username));
        Assert.AreEqual(JobStatus.Cancelled, _userJobScheduler.GetStatus(jobId, username));
    }

    [Test]
    [TestCase(TEST_USER, BASIC_USER)]
    public void AccessTest(string userWithAccess, string noAccessUser)
    {
        Guid jobId = _userJobScheduler.GetJobs(userWithAccess).FirstOrDefault();
        Guid nonExistentJobId = Guid.NewGuid();

        Assert.Throws<JobNotFoundException>(() => _userJobScheduler.CancelJob(nonExistentJobId, userWithAccess));
        Assert.Throws<JobAccessException>(() => _userJobScheduler.CancelJob(jobId, noAccessUser));
    }

    [Test]
    [TestCase(TEST_USER)]
    public void CancelFinishedJob(string username)
    {
        Guid jobId = _userJobScheduler.GetJobs(username).FirstOrDefault();

        while (_userJobScheduler.GetStatus(jobId, username) != JobStatus.Finished)
        {
            Task.Delay(TimeSpan.FromSeconds(1));
        }
        
        Assert.Throws<JobStatusException>(() => _userJobScheduler.CancelJob(jobId, username));
    }

    [Test]
    [TestCase(TEST_USER, BASIC_USER)]
    public void CancelJobWithTwoUsers(string jobOwner, string newUser)
    {
        Guid jobId = _userJobScheduler.GetJobs(jobOwner).FirstOrDefault();
        _userJobScheduler.AddToJob(jobId, newUser);
        
        Assert.DoesNotThrow(() => _userJobScheduler.CancelJob(jobId, jobOwner));
        Assert.AreNotEqual(JobStatus.Cancelled, _userJobScheduler.GetStatus(jobId, newUser));
    }
}