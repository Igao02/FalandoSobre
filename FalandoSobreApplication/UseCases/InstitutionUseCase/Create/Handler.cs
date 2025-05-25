using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.Extensions.Logging;


namespace FalandoSobreApplication.UseCases.InstitutionUseCase.Create;

public sealed class CreateInstitutionHandler : ICommandHandler<CreateInstitutionCommand, CreateInstitutionResponse>
{
    private readonly IInstitutionRepository _institutionRepository;
    private readonly ILogger<CreateInstitutionHandler> _logger;
    private readonly ILogRepository _logRepository;

    public CreateInstitutionHandler(IInstitutionRepository institutionRepository, ILogger<CreateInstitutionHandler> logger, ILogRepository logRepository)
    {
        _institutionRepository = institutionRepository;
        _logger = logger;
        _logRepository = logRepository;
    }

    public async Task<Result<CreateInstitutionResponse>> Handle(CreateInstitutionCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
        {
            var error = new Error("401", "Usuário não autenticado", ErrorType.Unauthorized);
            _logger.LogInformation("Usuário não autenticado: {Error}", error);
            return Result.Failure<CreateInstitutionResponse>(error);
        }

        try
        {
            var institution = new Institution(
                request.CorporateName,
                request.Document,
                request.Cep,
                request.City,
                request.Street,
                request.NumHome,
                request.Complement,
                DateTime.UtcNow,
                request.UserName,
                request.Neighborhood,
                request.Uf,
                request.ApplicationUserId,
                request.Actived
            );
            var createdInstitution = await _institutionRepository.AddAsync(institution);
            _logger.LogInformation("Instituição criada com ID: {InstitutionId}", createdInstitution.Id);

            var response = new CreateInstitutionResponse
            {
                Id = createdInstitution.Id,
                CorporateName = createdInstitution.CorporateName,
                Document = createdInstitution.Document,
                Cep = createdInstitution.Cep,
                City = createdInstitution.City,
                Street = createdInstitution.Street,
                NumHome = createdInstitution.NumHome,
                Complement = createdInstitution.Complement!,
                CreationDate = createdInstitution.CreationDate ?? DateTime.MinValue,
                UserName = createdInstitution.UserName!,
                Neighborhood = createdInstitution.Neighborhood,
                Uf = createdInstitution.Uf,
                ApplicationUserId = createdInstitution.ApplicationUserId,
                Actived = createdInstitution.Actived
            };

            var log = new Logs
            {
                Action = "Instituição criada com sucesso",
                ApplicationUserId = request.ApplicationUserId,
                Created_At = DateTime.UtcNow,
                EntityType = "Institution",
                UserName = request.UserName,
            };
            await _logRepository.Create(log);

            return Result.Success(response);
        }
        catch
        {
            var error = new Error("500", "Erro ao criar a instituição", ErrorType.Failure);
            _logger.LogInformation("Erro ao criar o relatório: {Error}", error);
            return Result.Failure<CreateInstitutionResponse>(error);
        }
    }
}
