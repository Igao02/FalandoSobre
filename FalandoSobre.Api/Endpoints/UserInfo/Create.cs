
using FalandoSobre.Api.Extensions;
using FalandoSobre.Api.Infrastructure;
using FalandoSobre.SharedKernel;
using FalandoSobreApplication.UseCases.UserInfoUseCase.Create;
using MediatR;

namespace FalandoSobre.Api.Endpoints.UserInfo;

public sealed class CreateUserInfoEndpoint : IEndpoint
{
    public sealed record Request(string ProfilePhoto, string ApplicationUserId, byte[] ProfilePhotoBytes);

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/user-info/create", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateUserInfoCommand(
                request.ProfilePhoto,
                request.ApplicationUserId,
                request.ProfilePhotoBytes);

            Result<CreateUserInfoResponse> result = await sender.Send(command, cancellationToken);
            return result.Match(
                value => Results.Ok(value),
                CustomResults.SimpleError
            );
        })
        .WithName("CreateUserInfo")
        .WithTags("UserInfo");
    }
}
