
using FalandoSobre.Api.Extensions;
using FalandoSobre.Api.Infrastructure;
using FalandoSobre.SharedKernel;
using FalandoSobreApplication.UseCases.UserInfoUseCase.List;
using MediatR;

namespace FalandoSobre.Api.Endpoints.UserInfo;

public class ListUserInfoEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/list-user-info", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new ListUserInfoCommand();
            Result<List<ListUserInfoResponse>> result = await sender.Send(command, cancellationToken);
            return result.Match(
                value => Results.Ok(value),
                CustomResults.SimpleError
            );
        })
        .WithName("ListUserInfos")
        .WithTags("UserInfo");
    }
}
