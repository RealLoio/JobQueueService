using JobQueueService.Exceptions;
using JobQueueService.Models;
using JobQueueService.Models.Jobs;
using JobQueueService.Services;
using JobService.Tests.TestJobs;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SharpDocxTemplateModels;

namespace JobService.Tests.JobSchedulerTests;

public class JobsInformationAcquiring
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
    public void JobStatusTest(string username)
    {
        Guid existingJobId = _userJobScheduler.GetJobs(username).First();
        
        Assert.DoesNotThrow(() => _userJobScheduler.GetStatus(existingJobId, username));
    }
    
    [Test]
    [TestCase(TEST_USER)]
    public void JobInformationTest(string username)
    {
        Guid jobId = _userJobScheduler.GetJobs(username).First();
        JobInformation jobInformation = _userJobScheduler.GetJobInformation(jobId, username);
        
        Assert.DoesNotThrow(() => _userJobScheduler.GetJobInformation(jobId, username));
        Assert.AreEqual(jobInformation.Status, _userJobScheduler.GetStatus(jobId, username));
    }
    
    [Test]
    [TestCase(TEST_USER)]
    public void JobInformationDataTest(string username)
    {
        Guid jobId = _userJobScheduler.GetJobs(username).First();
        
        while (_userJobScheduler.GetStatus(jobId, username) != JobStatus.Finished)
        {
            Task.Delay(TimeSpan.FromSeconds(1));
        }
        
        JobInformation jobInformation = _userJobScheduler.GetJobInformation(jobId, username);
        
        Assert.AreEqual(jobInformation.Status, _userJobScheduler.GetStatus(jobId, username));
        Assert.AreEqual(jobId, jobInformation.JobId);
        Assert.IsTrue(!String.IsNullOrEmpty(jobInformation.Description));
        Assert.IsNotNull(jobInformation.PostedAt);
        Assert.IsNotNull(jobInformation.StartedAt);
        Assert.IsNotNull(jobInformation.FinishedAt);
    }

    [Test]
    [TestCase(TEST_USER, BASIC_USER)]
    public void AccessTest(string userWithAccess, string noAccessUser)
    {
        Guid nonExistentJobId = Guid.NewGuid();
        Guid jobId = _userJobScheduler.GetJobs(userWithAccess).FirstOrDefault();

        Assert.Throws<JobNotFoundException>(() => _userJobScheduler.GetStatus(nonExistentJobId, userWithAccess));
        Assert.Throws<JobAccessException>(() => _userJobScheduler.GetStatus(jobId, noAccessUser));
        Assert.Throws<JobAccessException>(() => _userJobScheduler.GetJobInformation(jobId, noAccessUser));
    }
}