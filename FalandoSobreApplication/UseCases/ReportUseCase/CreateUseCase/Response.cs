namespace FalandoSobre.Application.UseCases.ReportUseCase.CreateUseCase;

public sealed record Response(Guid Id)
{
    public bool IsSuccess { get; set; }
}
