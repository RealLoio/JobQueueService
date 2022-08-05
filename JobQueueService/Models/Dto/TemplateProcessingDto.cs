using SharpDocxTemplateModels;

namespace JobQueueService.Models.Dto;

public class TemplateProcessingDto
{
    public UniversalApplicationModel UniversalApplicationModel { get; set; }
    public TemplatePayloadModel TemplatePayloadModel { get; set; }
    public string Description { get; set; }
}