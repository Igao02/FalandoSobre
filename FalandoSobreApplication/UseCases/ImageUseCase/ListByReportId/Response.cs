namespace FalandoSobreApplication.UseCases.ImageUseCase.ListByReportId;

public sealed class ImageListByReportIdResponse 
{
    public Guid Id { get; init; }
    
    public string? ImageUrl { get; init; }
    
    public Guid? ReportId { get; init; }
}