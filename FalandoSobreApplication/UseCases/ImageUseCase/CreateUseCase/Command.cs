using Application.Abstractions.Messaging;

namespace FalandoSobreApplication.UseCases.ImageUseCase.CreateUseCase;

public sealed record class CreateImageCommand(string ImageUrl, byte[] ConteudoArquivo, DateTime ImageDate, Guid ReportId)
    : ICommand<CreateImageResponse>;
