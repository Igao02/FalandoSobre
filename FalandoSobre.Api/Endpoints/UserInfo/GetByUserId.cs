
using FalandoSobre.Api.Extensions;
using FalandoSobre.Api.Infrastructure;
using FalandoSobre.SharedKernel;
using FalandoSobreApplication.UseCases.UserInfoUseCase.GetByUseId;
using MediatR;

namespace FalandoSobre.Api.Endpoints.UserInfo;

public sealed class GetByUserId : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/user-info/user/", async (string applicationUserId, ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new GetByUserIdCommand(applicationUserId);

                Result<GetByUserIdResponse> result = await sender.Send(command, cancellationToken);
                return result.Match(value => Results.Ok(value), CustomResults.SimpleError);
            })
            .WithName("GetByUserId")
            .WithTags("UserInfo");
    }
}
