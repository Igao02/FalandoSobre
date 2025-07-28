using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.Extensions.Logging;

namespace FalandoSobreApplication.UseCases.UserInfoUseCase.Create;

public sealed class CreateUserInfoHandler : ICommandHandler<CreateUserInfoCommand, CreateUserInfoResponse>
{
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly ILogger<CreateUserInfoHandler> _logger;
    private readonly ILogRepository _logRepository;

    public CreateUserInfoHandler(IUserInfoRepository userInfoRepository, ILogger<CreateUserInfoHandler> logger, ILogRepository logRepository)
    {
        _userInfoRepository = userInfoRepository;
        _logger = logger;
        _logRepository = logRepository;
    }

    public async Task<Result<CreateUserInfoResponse>> Handle(CreateUserInfoCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ApplicationUserId))
        {
            var error = new Error("401", "Usuário não autenticado", ErrorType.Unauthorized);
            _logger.LogInformation("Usuário não autenticado: {Error}", error);
            return Result.Failure<CreateUserInfoResponse>(error);
        }

        _logger.LogInformation("Iniciando criação do usuário com ID: {ApplicationUserId}", request.ApplicationUserId);

        try
        {
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ReportImages", "Uploads");
            Directory.CreateDirectory(uploadFolder);

            var fileName = $"{Guid.NewGuid()}.png";
            var filePath = Path.Combine(uploadFolder, fileName);

            await File.WriteAllBytesAsync(filePath, request.ProfilePhotoBytes, cancellationToken);
            var profilePhotoUrl = $"https://localhost:7249/ReportImages/Uploads/{fileName}";

            var userInfo = new UserInfo
            {
                Actived = true,
                ProfilePhoto = profilePhotoUrl,
                ApplicationUserId = request.ApplicationUserId,
                CreatedAt = DateTime.UtcNow,
                ProfilePhotoBytes = request.ProfilePhotoBytes,
            };

            var createdUserInfo = await _userInfoRepository.AddAsync(userInfo);
            _logger.LogInformation("Informação adicional do usuário criada com ID: {UserInfoId}", createdUserInfo.Id);

            var response = new CreateUserInfoResponse
            {
                Id = createdUserInfo.Id,
                ProfilePhoto = createdUserInfo.ProfilePhoto,
                CreatedAt = createdUserInfo.CreatedAt ?? DateTime.UtcNow,
                Actived = createdUserInfo.Actived ?? true,
                ApplicationUserId = createdUserInfo.ApplicationUserId
            };

            var log = new Logs
            {
                Action = "Informação adicional do usuário criada com sucesso",
                Created_At = DateTime.UtcNow,
                EntityType = "User",
                ApplicationUserId = request.ApplicationUserId,
            };

            await _logRepository.Create(log);
            _logger.LogInformation("Log criado com sucesso: {Log}", log);

            return Result.Success(response);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar informação adicional usuário com ID: {ApplicationUserId}", request.ApplicationUserId);
            return Result.Failure<CreateUserInfoResponse>(new Error("500", "Erro interno do servidor", ErrorType.Failure));
        }
    }
}
