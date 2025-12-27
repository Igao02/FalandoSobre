using FalandoSobre.Api.Extensions;
using FalandoSobre.Api.Infrastructure;
using FalandoSobre.SharedKernel;
using FalandoSobreApplication.UseCases.LikeUseCase.ListByUserId;
using MediatR;

namespace FalandoSobre.Api.Endpoints.Like;

public sealed class ListLikesByUserIdEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/likes/user/{userId}", async (string userId, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new ListLikesByUserIdCommand(userId);

            Result<ListLikesByUserIdResponse> result = await sender.Send(command, cancellationToken);
            return result.Match(
                value => Results.Ok(value),
                CustomResults.SimpleError
            );
        })
        .WithName("ListLikesByUserId")
        .WithTags("Like");
    }
}
