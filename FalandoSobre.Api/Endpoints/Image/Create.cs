using MediatR;
using FalandoSobre.Api.Extensions;
using FalandoSobre.SharedKernel;
using FalandoSobre.Api.Infrastructure;
using FalandoSobreApplication.UseCases.ImageUseCase.CreateUseCase;

namespace FalandoSobre.Api.Endpoints.Image;

public sealed class CreateImageEndpoint : IEndpoint
{
    public sealed record Request(string ImageUrl, byte[] ConteudoArquivo, DateTime CreatedAt, Guid ReportId, string ApplicationUserId);

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/images/create", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateImageCommand(
                request.ImageUrl,
                request.ConteudoArquivo,
                request.CreatedAt,
                request.ReportId,
                request.ApplicationUserId);

            Result<CreateImageResponse> result = await sender.Send(command, cancellationToken);
            return result.Match(
                value => Results.Ok(value),
                CustomResults.SimpleError
            );
        })
            .WithName("CreateImage")
            .WithTags("Images");
    }
}
