using JobQueueService.Models;
using JobQueueService.Models.Dto;
using JobQueueService.Models.Jobs;
using JobQueueService.Services;
using JobService.Tests.TestJobs;
using JobService.Tests.TestServices;
using Microsoft.Extensions.Logging;
using SharpDocxTemplateModels;

namespace JobService.Tests;
public static class TestsHelper
{
    public const string SKIP_SETUP = "SkipSetup";
    public static readonly UniversalApplicationModel Data =  new("test", OutputFileFormat.pdf, new []{"test"});
    public static readonly string TestUser = "Test";
    public static readonly string BasicUser = "BasicUser";
    public static readonly string Description = "Test job";

    public static TemplateProcessingDto CreateDto(string methodName, string username, int number = 0)
    {
        TemplateProcessingDto dto = new()
        {
            TemplatePayloadModel = GetPayload(methodName, username, number),
            UniversalApplicationModel = Data,
            Description = Description
        };
        return dto;
    }

    public static JobInput<UniversalApplicationModel> JobInput(TemplatePayloadModel payload)
    {
        return new JobInput<UniversalApplicationModel>(Data, "Test task", payload);
    }

    public static TemplateProcessingDto CreateDto(TemplatePayloadModel payloadModel)
    {
        TemplateProcessingDto dto = new()
        {
            TemplatePayloadModel = payloadModel,
            UniversalApplicationModel = Data,
            Description = Description
        };
        return dto;
    }

    public static TemplatePayloadModel GetPayload(string methodName, string username, int number = 0)
    {
        TemplatePayloadModel templatePayloadModel = 
            new($"{methodName}.{username}.{number}", "test", "test", "test");
        return templatePayloadModel;
    }

    public static TestProcessingService CreateService()
    {
        return new TestProcessingService();
    }

    public static UserJobScheduler<UniversalApplicationModel, string> CreateScheduler(ITestJobBase job)
    {
        JobWrapper<UniversalApplicationModel, string> jobWrapper = new(job.ExecuteJobAsync);
        UserJobScheduler<UniversalApplicationModel, string> scheduler = new(jobWrapper, new LoggerFactory());
        return scheduler;
    }
}