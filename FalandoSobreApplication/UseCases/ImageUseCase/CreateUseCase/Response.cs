namespace FalandoSobreApplication.UseCases.ImageUseCase.CreateUseCase;

public sealed class CreateImageResponse
{
    public Guid Id { get; init; }
    public string ImageUrl { get; init; } = default!;
    public byte[]? ConteudoArquivo { get; init; } = default!;
    public DateTime ImageDate { get; init; } = DateTime.Now;
    public Guid ReportId { get; init; } = default!;
    public string ApplicationUserId { get; init; } = default!;

}
