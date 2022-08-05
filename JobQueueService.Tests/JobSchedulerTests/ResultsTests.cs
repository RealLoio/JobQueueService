using JobQueueService.Exceptions;
using JobQueueService.Models;
using JobQueueService.Models.Jobs;
using JobQueueService.Services;
using JobService.Tests.TestJobs;
using NUnit.Framework;
using SharpDocxTemplateModels;

namespace JobService.Tests.JobSchedulerTests;

public class ResultsTests
{
    private UserJobScheduler<UniversalApplicationModel, string> _userJobScheduler;
    private const string TEST_USER = nameof(TestsHelper.TestUser);
    private const string BASIC_USER = nameof(TestsHelper.BasicUser);
    private const int JOBS_FOR_EACH_USER_COUNT = 3;
    private readonly string[] _users = {TEST_USER, BASIC_USER};
    
    [SetUp]
    public void SetUpTheTest()
    {
        JobThatEndsQuick newJobAsync = new();
        _userJobScheduler = TestsHelper.CreateScheduler(newJobAsync);
        
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
    public void GetUnfinishedJobResult(string username)
    {
        Guid jobId = _userJobScheduler.GetJobs(username).FirstOrDefault();

        Assert.Throws<JobStatusException>(() => _userJobScheduler.GetResult(jobId, username));
    }

    [Test]
    [TestCase(TEST_USER, BASIC_USER)]
    public void AccessTest(string userWithAccess, string noAccessUser)
    {
        Guid jobId = _userJobScheduler.GetJobs(userWithAccess).FirstOrDefault();
        Guid nonExistentJobId = Guid.NewGuid();

        while (_userJobScheduler.GetStatus(jobId, userWithAccess) != JobStatus.Finished)
        {
            Task.Delay(500);
        }
        
        Assert.Throws<JobAccessException>(() => _userJobScheduler.GetResult(jobId, noAccessUser));
        Assert.DoesNotThrow(() => _userJobScheduler.GetResult(jobId, userWithAccess));
        Assert.Throws<JobAccessException>(() => _userJobScheduler.GetResult(jobId, userWithAccess));
        Assert.Throws<JobNotFoundException>(() => _userJobScheduler.GetResult(nonExistentJobId, userWithAccess));
    }

    [Test]
    [TestCase(TEST_USER)]
    public void GetResults(string username)
    {
        IEnumerable<Guid> jobIds = _userJobScheduler.GetJobs(username);
        string result = String.Empty;
        
        foreach (Guid jobId in jobIds)
        {
            while (_userJobScheduler.GetStatus(jobId, username) != JobStatus.Finished)
            {
                Task.Delay(TimeSpan.FromSeconds(1));
            }
            
            Assert.DoesNotThrow(() => result = _userJobScheduler.GetResult(jobId, username));
            Assert.IsNotEmpty(result);
        }
    }
}