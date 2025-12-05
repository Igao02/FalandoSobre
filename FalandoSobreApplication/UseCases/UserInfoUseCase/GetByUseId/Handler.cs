using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.Extensions.Logging;

namespace FalandoSobreApplication.UseCases.UserInfoUseCase.GetByUseId;

public sealed class GetByUserIdHandler(IUserInfoRepository userInfoRepository, ILogger<GetByUserIdHandler> logger) : ICommandHandler<GetByUserIdCommand, GetByUserIdResponse>
{
    public async Task<Result<GetByUserIdResponse>> Handle(GetByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userInfo = await userInfoRepository.GetImageByUserId(request.ApplicationUserId);

            if (userInfo is null)
            {
                var emptyResponse = new GetByUserIdResponse
                {
                    Id = Guid.Empty,
                    ProfilePhoto = null,
                    ApplicationUserId = request.ApplicationUserId,
                };
                return Result.Success(emptyResponse);
            }

            var response = new GetByUserIdResponse
            {
                Id = userInfo.Id,
                ProfilePhoto = userInfo.ProfilePhoto,
                ApplicationUserId = userInfo.ApplicationUserId,
            };

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            var error = new Error("500", "Erro ao buscar informações do usuário", ErrorType.NotFound);
            logger.LogError(ex, "Erro ao buscar informações do usuário: {Error}", error);
            return Result.Failure<GetByUserIdResponse>(error);
        }
    }
}
