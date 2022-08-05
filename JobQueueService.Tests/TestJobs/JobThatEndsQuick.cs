using SharpDocxTemplateModels;

namespace JobService.Tests.TestJobs;

public class JobThatEndsQuick : ITestJobBase
{
    public async Task<string> ExecuteJobAsync(UniversalApplicationModel data, CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(100), token);
        return $"Fake result for {data.TemplateName}";
    }
}