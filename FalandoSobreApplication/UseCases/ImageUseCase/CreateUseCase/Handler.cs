using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.Extensions.Logging;

namespace FalandoSobreApplication.UseCases.ImageUseCase.CreateUseCase;

public sealed class CreateImageHandler : ICommandHandler<CreateImageCommand, Guid>
{
    private readonly IImageRepository _imageRepository;
    private readonly ILogger<CreateImageHandler> _logger;

    public CreateImageHandler(IImageRepository imageRepository, ILogger<CreateImageHandler> logger)
    {
        _imageRepository = imageRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Iniciando criação da imagem para o relatório {ReportId}", request.ReportId);

            var fileName = $"png"; // ou .jpg, etc, dependendo do tipo da imagem
            _logger.LogInformation($"File name aqui? {fileName}");
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ReportImages", "Uploads");
            Directory.CreateDirectory(uploadFolder); // garante que a pasta exista
            var filePath = Path.Combine(uploadFolder, fileName);
            _logger.LogInformation("Imagem salva no disco: {FilePath}", filePath);

            await File.WriteAllBytesAsync(filePath, request.ConteudoArquivo, cancellationToken);

            var imageUrl = $"/ReportImages/Uploads/{fileName}"; // isso sim vai no banco!
            _logger.LogInformation("URL da imagem: {ImageUrl}", imageUrl);

            _logger.LogInformation("ReportId antes de entrar aqui para criar novo{request.ReportId}", request.ReportId);

            var image = new Image(
                imageUrl: imageUrl,
                conteudoArquivo: null, // se quiser, pode salvar só no disco e deixar null no banco
                imageDate: request.CreatedAt,
                reportId: request.ReportId
            );
            _logger.LogInformation("Criando imagem com URL {image}", image);

            var result = await _imageRepository.AddImageAsync(image);

            _logger.LogInformation("Imagem criada com sucesso. ID: {ImageId}", result.Id);

            return Result.Success(result.Id);
        }
        catch (Exception ex)
        {
            var error = new Error("500", "Erro ao criar a imagem", 0);
            _logger.LogError(ex, "Erro ao criar a imagem: {Error}", error);
            return Result.Failure<Guid>(error);
        }
    }
}
