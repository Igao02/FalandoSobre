namespace FalandoSobreApplication.UseCases.SharedReportUseCase.Create;

public sealed class SharedReportResponse
{
    public Guid Id { get; init; }
    public Guid ReportId { get; init; }
    public string? ApplicationUserId { get; init; }
    public bool Actived { get; init; }
    public DateTime CreatedAt { get; init; }
}
