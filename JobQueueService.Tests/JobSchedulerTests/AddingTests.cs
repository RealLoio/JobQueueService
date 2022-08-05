using JobQueueService.Exceptions;
using JobQueueService.Models;
using JobQueueService.Services;
using JobService.Tests.TestJobs;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SharpDocxTemplateModels;

namespace JobService.Tests.JobSchedulerTests;

public class AddingTests
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
    [TestCase(TEST_USER, BASIC_USER)]
    [TestCase(BASIC_USER, TEST_USER)]
    public void AddingOtherUserJobTest(string username, string otherUser)
    {
        IEnumerable<Guid> otherUserJobs = _userJobScheduler.GetJobs(otherUser);
        TemplatePayloadModel otherUserJob = TestsHelper.GetPayload(nameof(SetUpTheTest), otherUser);
        Guid otherUserJobId = otherUserJob.GetUniqueIdentifier();
        Guid userJobId = Guid.Empty;
        
        Assert.Throws<JobAccessException>(() => _userJobScheduler.GetStatus(otherUserJobId, username));
        Assert.DoesNotThrow(() => userJobId = _userJobScheduler.AddJob(TestsHelper.JobInput(otherUserJob), username));
        Assert.AreEqual(otherUserJobId, userJobId);
        Assert.IsTrue(otherUserJobs.Contains(userJobId));
    }
}