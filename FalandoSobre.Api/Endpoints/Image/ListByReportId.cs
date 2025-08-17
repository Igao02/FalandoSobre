using FalandoSobre.Api.Extensions;
using FalandoSobre.Api.Infrastructure;
using FalandoSobre.SharedKernel;
using FalandoSobreApplication.UseCases.ImageUseCase.ListByReportId;
using MediatR;

namespace FalandoSobre.Api.Endpoints.Image;

public sealed class ListByReportId : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/images/reports/", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new ImageListByReportIdCommand(id);

                Result<List<ImageListByReportIdResponse>> result = await sender.Send(command, cancellationToken);
                return result.Match(value => Results.Ok(value), CustomResults.SimpleError);
            })
            .WithName("ImageListByReportId")
            .WithTags("Images")
            .WithOpenApi();
    }
}