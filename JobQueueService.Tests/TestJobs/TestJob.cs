using Microsoft.Extensions.Logging;
using SharpDocxTemplateModels;

namespace JobService.Tests.TestJobs;

public class TestJob : ITestJobBase
{
    private readonly ILogger<TestJob> _logger;

    public TestJob(ILoggerFactory factory)
    {
        _logger = factory.CreateLogger<TestJob>();
    }

    public async Task<string> ExecuteJobAsync(UniversalApplicationModel data, CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), token);
        return $"Fake result for {data.TemplateName}";
    }
}