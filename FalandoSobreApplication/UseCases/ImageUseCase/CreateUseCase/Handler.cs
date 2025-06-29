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
    private readonly ILogRepository _logRepository;

    public CreateImageHandler(IImageRepository imageRepository, ILogger<CreateImageHandler> logger, ILogRepository logRepository )
    {
        _imageRepository = imageRepository;
        _logger = logger;
        _logRepository = logRepository;
    }

    public async Task<Result<CreateImageResponse>> Handle(CreateImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request == null)
            {
                var error = new Error("400", "Requisição inválida", 0);
                _logger.LogWarning("Requisição nula recebida.");
                return Result.Failure<CreateImageResponse>(error);
            }

            if (string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                var error = new Error("400", "O nome do arquivo da imagem não pode ser vazio", 0);
                _logger.LogWarning("Nome do arquivo de imagem ausente.");
                return Result.Failure<CreateImageResponse>(error);
            }

            if (request.ReportId == Guid.Empty)
            {
                var error = new Error("400", "O ID da publicação é inválido", 0);
                _logger.LogWarning("ID da publicação inválido: {ReportId}", request.ReportId);
                return Result.Failure<CreateImageResponse>(error);
            }

            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ReportImages", "Uploads");

            _logger.LogInformation("Criando diretório para upload: {UploadFolder}", uploadFolder);
            Directory.CreateDirectory(uploadFolder);

            
            var fileName = Path.GetFileName(request.ImageUrl); 
            var filePath = Path.Combine(uploadFolder, fileName); 

            await File.WriteAllBytesAsync(filePath, request.ConteudoArquivo, cancellationToken);
            
            var imageUrl = "https://localhost:7249/ReportImages/Uploads/" + fileName;

            var image = new Image(
                imageUrl: imageUrl,
                conteudoArquivo: null,
                imageDate: DateTime.Now,
                reportId: request.ReportId,
                applicationUserId: request.ApplicationUserId
            );
            _logger.LogInformation("Criando imagem {image}", image);

            var result = await _imageRepository.AddImageAsync(image);

            _logger.LogInformation("Imagem criada com sucesso. ID: {ImageId}", result.Id);

            var response = new CreateImageResponse
            {
                Id = result.Id,
                ImageUrl = result.ImageUrl,
                ConteudoArquivo = null,
                ImageDate = result.ImageDate,
                ReportId = result.ReportId,
                ApplicationUserId = result.ApplicationUserId!
            };
            _logger.LogInformation("Resposta da criação da imagem: {Response}", response);

            var log = new Logs
            {
                Action = "Imagem criada com sucesso",
                Created_At = DateTime.UtcNow,
                EntityType = "Image",
                ApplicationUserId = request.ApplicationUserId,
                UserName = ""
            };

            await _logRepository.Create(log);

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
