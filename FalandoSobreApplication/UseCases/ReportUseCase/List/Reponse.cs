namespace FalandoSobreApplication.UseCases.ReportUseCase.ListUseCase;

public sealed class ListReportReponse
{
    public Guid Id { get; init; }
    public string ReportName { get; init; } = default!;
    public string TypeReport { get; init; } = default!;
    public string ReportDescription { get; init; } = default!;
    public DateTime ReportDate { get; init; }
    public string UserName { get; init; } = default!;
    public bool IsEvent { get; init; } = default!;
    public string ApplicationUserId { get; init; } = default!;
    public bool Actived { get; init; } = default!;

}
