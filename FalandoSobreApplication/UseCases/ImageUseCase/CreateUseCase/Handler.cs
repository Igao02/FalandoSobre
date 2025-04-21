using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.Extensions.Logging;

namespace FalandoSobreApplication.UseCases.ImageUseCase.CreateUseCase;

public sealed class CreateImageHandler : ICommandHandler<CreateImageCommand, CreateImageResponse>
{
    private readonly IImageRepository _imageRepository;
    private readonly ILogger<CreateImageHandler> _logger;

    public CreateImageHandler(IImageRepository imageRepository, ILogger<CreateImageHandler> logger)
    {
        _imageRepository = imageRepository;
        _logger = logger;
    }

    public async Task<Result<CreateImageResponse>> Handle(CreateImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ReportImages", "Uploads"); ;


            _logger.LogInformation("Criando diretório para upload: {UploadFolder}", uploadFolder);
            Directory.CreateDirectory(uploadFolder);


            var filePath = Path.Combine(uploadFolder, request.ImageUrl);

            await File.WriteAllBytesAsync(filePath, request.ConteudoArquivo, cancellationToken);

            //var imageUrl = $"/ReportImages/Uploads/{request.ImageUrl}"; 

            var image = new Image(
                imageUrl: filePath,
                conteudoArquivo: null,
                imageDate: DateTime.Now,
                reportId: request.ReportId
            );
            _logger.LogInformation("Criando imagem {image}", image);

            var result = await _imageRepository.AddImageAsync(image);

            _logger.LogInformation("Imagem criada com sucesso. ID: {ImageId}", result);

            var response = new CreateImageResponse
            {
                Id = result.Id,
                ImageUrl = result.ImageUrl,
                ConteudoArquivo = null,
                ImageDate = result.ImageDate,
                ReportId = result.ReportId
            };
            _logger.LogInformation("Resposta da criação da imagem: {Response}", response);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            var error = new Error("500", "Erro ao criar a imagem", 0);
            _logger.LogError(ex, "Erro ao criar a imagem: {Error}", error);
            return Result.Failure<CreateImageResponse>(error);
        }
    }
}
