using SharpDocxTemplateModels;

namespace JobService.Tests.TestJobs;

public class JobWithNoResult : ITestJobBase
{
    public async Task<string> ExecuteJobAsync(UniversalApplicationModel data, CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(100), token);
        return null!;
    }
}