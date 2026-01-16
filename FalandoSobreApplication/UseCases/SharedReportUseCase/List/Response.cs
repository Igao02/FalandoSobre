namespace FalandoSobreApplication.UseCases.SharedReportUseCase.List;

public class ListSharedReportResponse
{
    public Guid Id { get; init; }
    public Guid ReportId { get; init; }
    public string? ApplicationUserId { get; init; }
    public bool Actived { get; init; }
    public DateTime CreatedAt { get; init; }
}

