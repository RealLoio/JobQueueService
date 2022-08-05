using SharpDocxTemplateModels;

namespace JobService.Tests.TestJobs;

public class JobThatFails : ITestJobBase
{
    public async Task<string> ExecuteJobAsync(UniversalApplicationModel data, CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromSeconds(1), token);
        string nullString = null!;
        int length = nullString!.Length;
        return length.ToString();
    }
}