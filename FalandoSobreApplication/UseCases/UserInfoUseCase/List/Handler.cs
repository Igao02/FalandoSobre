using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.Extensions.Logging;

namespace FalandoSobreApplication.UseCases.UserInfoUseCase.List;

public sealed class ListUserInfoHandler(IUserInfoRepository userInfoRepository, ILogger<ListUserInfoHandler> logger) : ICommandHandler<ListUserInfoCommand, List<ListUserInfoResponse>>
{
    public async Task<Result<List<ListUserInfoResponse>>> Handle(ListUserInfoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userInfos = await userInfoRepository.GetAllAsync();

            var response = userInfos.Select(userInfo => new ListUserInfoResponse
            {
                Id = userInfo!.Id,
                ProfilePhoto = userInfo.ProfilePhoto,
                ApplicationUserId = userInfo.ApplicationUserId,
                Actived = userInfo.Actived ?? false,
                CreatedAt = userInfo.CreatedAt ?? DateTime.MinValue
            }).ToList();

            return Result.Success(response);
        }
        catch (Exception ex) 
        {
            logger.LogError(ex, "Erro ao listar os compartilhamentos");
            var error = new Error("500", "Erro ao listar as informações dos usuários", ErrorType.Failure);
            return Result.Failure<List<ListUserInfoResponse>>(error);
        }
    }
}
