namespace FalandoSobreApplication.UseCases.ReportUseCase.Delete;

public sealed class DeleteReportResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}
