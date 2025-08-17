using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using FalandoSobreApplication.UseCases.ImageUseCase.ListByReportId;
using Microsoft.Extensions.Logging;

public sealed class ListImageByReportIdHandler(
    IImageRepository imageRepository,
    ILogger<ListImageByReportIdHandler> logger)
    : ICommandHandler<ImageListByReportIdCommand, List<ImageListByReportIdResponse>>
{
    public async Task<Result<List<ImageListByReportIdResponse>>> Handle(
        ImageListByReportIdCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var images = await imageRepository.GetImageByReportId(request.Id);

            if (images is null || !images.Any())
                return Result.Success(new List<ImageListByReportIdResponse>());

            var response = images
               .Select(img => new ImageListByReportIdResponse
               {
                   Id = img.Id,
                   ImageUrl = img.ImageUrl,
                   ReportId = img.ReportId
               })
               .ToList();


            logger.LogInformation("Imagens listadas com sucesso: {Count} imagens encontradas", response.Count);

            return Result.Success(response);
        }
        catch (Exception e)
        {
            var error = new Error("500", "Erro ao listar as imagens", ErrorType.NotFound);
            logger.LogError(e, "Erro ao listar imagens: {Error}", error);
            return Result.Failure<List<ImageListByReportIdResponse>>(error);
        }
    }
}
