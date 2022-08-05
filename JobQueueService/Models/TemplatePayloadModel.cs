using JobQueueService.Helpers;

namespace JobQueueService.Models;

public sealed class TemplatePayloadModel : IUniquePayload
{
    public string ApplicationName { get; set; }
    public string TemplateName { get; set; }
    public string QueryName { get; set; }
    public string? SpecialQueueName { get; set; }
    
    public TemplatePayloadModel(string applicationName, string templateName, string queryName, string? queueName = null)
    {
        this.ApplicationName = applicationName;
        this.TemplateName = templateName;
        this.QueryName = queryName;
        this.SpecialQueueName = queueName;
    }

    public override string ToString()
    {
        string generatedString = $"{this.ApplicationName}.{this.TemplateName}.{this.QueryName}";
        if (!String.IsNullOrEmpty(this.SpecialQueueName))
        {
            generatedString += $".{this.SpecialQueueName}";
        }
        return generatedString;
    }

    public Guid GetUniqueIdentifier()
    {
        return IdGenerationHelper.GenerateGuid(ToString());
    }
}