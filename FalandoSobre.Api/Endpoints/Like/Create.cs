
using FalandoSobre.Api.Extensions;
using FalandoSobre.Api.Infrastructure;
using FalandoSobre.SharedKernel;
using FalandoSobreApplication.UseCases.LikeUseCase.Create;
using MediatR;

namespace FalandoSobre.Api.Endpoints.Like;

public sealed class CreateLikeEndpoint : IEndpoint
{
    public sealed record Request(Guid ReportId, string? ApplicationUserId);

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/create-like", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateLikeCommand(
                request.ReportId,
                request.ApplicationUserId);

                Result<LikeResponse> result = await sender.Send(command, cancellationToken);
                return result.Match(
                    value => Results.Ok(value),
                    CustomResults.SimpleError
            );
        })
        .WithName("CreateLike")
        .WithTags("Like")
        .WithOpenApi();
    }
}
