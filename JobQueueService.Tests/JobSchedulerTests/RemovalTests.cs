using JobQueueService.Exceptions;
using JobQueueService.Models;
using JobQueueService.Services;
using JobService.Tests.TestJobs;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SharpDocxTemplateModels;

namespace JobService.Tests.JobSchedulerTests;

public class RemovalTests
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
    [TestCase(BASIC_USER)]
    public void RemoveJob(string username)
    {
        Guid jobId = _userJobScheduler.GetJobs(username).FirstOrDefault();
        Guid nonExistentJobId = Guid.NewGuid();
        
        Assert.Throws<JobNotFoundException>(() => _userJobScheduler.RemoveJob(nonExistentJobId, username));
        Assert.DoesNotThrow(() => _userJobScheduler.RemoveJob(jobId, username));
        Assert.Throws<JobNotFoundException>(() => _userJobScheduler.GetStatus(jobId, username));
    }

    [Test]
    [TestCase(TEST_USER, BASIC_USER)]
    [TestCase(BASIC_USER, TEST_USER)]
    public void AccessTest(string userWithAccess, string noAccessUser)
    {
        Guid jobId = _userJobScheduler.GetJobs(userWithAccess).FirstOrDefault();
        TemplatePayloadModel templatePayloadModel = TestsHelper.GetPayload(nameof(SetUpTheTest), noAccessUser);
        Guid jobWithManySubscribers = _userJobScheduler.AddJob(TestsHelper.JobInput(templatePayloadModel), userWithAccess);
        
        Assert.Throws<JobAccessException>(() => _userJobScheduler.RemoveJob(jobId, noAccessUser));
        Assert.DoesNotThrow(() => _userJobScheduler.RemoveJob(jobWithManySubscribers, userWithAccess));
        Assert.Throws<JobAccessException>(() => _userJobScheduler.GetStatus(jobWithManySubscribers, userWithAccess));
    }

    [Test]
    [TestCase(TEST_USER)]
    [TestCase(BASIC_USER)]
    public void JobsRemovalTest(string username)
    {
        IEnumerable<Guid> jobIds = _userJobScheduler.GetJobs(username);

        Assert.DoesNotThrow(() => _userJobScheduler.RemoveJobs(jobIds, username));
        foreach (Guid jobId in jobIds)
        {
            Assert.Throws<JobNotFoundException>(() => _userJobScheduler.GetStatus(jobId, username));
        }
    }
}