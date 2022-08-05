using JobQueueService.Exceptions;
using JobQueueService.Models;
using JobQueueService.Repositories;
using NUnit.Framework;

namespace JobService.Tests.RepositoriesTests;

public class UsersRepositoryTests
{
    private IUsersRepository _usersRepository;
    private const string TEST_USER = nameof(TestsHelper.TestUser);
    private const string BASIC_USER = nameof(TestsHelper.BasicUser);
    private const int JOBS_FOR_EACH_USER_COUNT = 3;
    private readonly string[] _users = {TEST_USER, BASIC_USER};

    [SetUp]
    public void SetUpTheTest()
    {
        _usersRepository = new UserRepository();
        
        foreach (string user in _users)
        {
            for (int i = 0; i < JOBS_FOR_EACH_USER_COUNT; i++)
            {
                TemplatePayloadModel templatePayloadModel = TestsHelper.GetPayload(nameof(SetUpTheTest), user, i);
                Guid jobId = templatePayloadModel.GetUniqueIdentifier();
                _usersRepository.AddToJob(jobId, user);
            }
        }
    }

    [Test]
    [TestCase(TEST_USER)]
    public void AddSameJobTest(string username)
    {
        TemplatePayloadModel job = TestsHelper.GetPayload(nameof(SetUpTheTest), username);
        Guid jobId = job.GetUniqueIdentifier();
        
        Assert.IsTrue(_usersRepository.HasJob(jobId, username));
        Assert.DoesNotThrow(() => _usersRepository.AddToJob(jobId, username));
        Assert.DoesNotThrow(() => _usersRepository.RemoveFromJob(jobId, username));
        Assert.IsTrue(!_usersRepository.HasJob(jobId, username));
    }


    [Test]
    [TestCase(TEST_USER)]
    public void RemoveJobTwiceTest(string username)
    {
        TemplatePayloadModel job = TestsHelper.GetPayload(nameof(SetUpTheTest), username);
        Guid jobId = job.GetUniqueIdentifier();

        Assert.IsTrue(_usersRepository.HasJob(jobId, username));
        Assert.DoesNotThrow(() => _usersRepository.RemoveFromJob(jobId, username));
        Assert.IsTrue(!_usersRepository.HasJob(jobId, username));
        Assert.DoesNotThrow(() => _usersRepository.RemoveFromJob(jobId, username));
    }

    [Test]
    [TestCase(TEST_USER)]
    public void HasNonexistentJobTest(string username)
    {
        Guid nonexistentJobId = Guid.NewGuid();

        Assert.Throws<JobNotFoundException>(() => _usersRepository.HasJob(nonexistentJobId, username));
    }
    

}