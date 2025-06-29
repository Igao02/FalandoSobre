using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.SharedKernel;
using Microsoft.Extensions.Logging;

namespace FalandoSobreApplication.UseCases.ImageUseCase.ListByReportId;

public sealed class ListImageByReportIdHandler(
    IImageRepository imageRepository,
    ILogger<ListImageByReportIdHandler> logger)
    : ICommandHandler<ImageListByReportIdCommand, ImageListByReportIdResponse>
{
    public async Task<Result<ImageListByReportIdResponse>> Handle(ImageListByReportIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var image = await imageRepository.GetImageByReportId(request.Id);

            if (image is null)
            {
                var emptyResponse = new ImageListByReportIdResponse
                {
                    Id = Guid.Empty,
                    ImageUrl = null,
                    ReportId = request.Id
                };

                return Result.Success(emptyResponse);
            }

            var (id, imageUrl, reportId) = image.Value;

            var response = new ImageListByReportIdResponse
            {
                Id = id,
                ImageUrl = imageUrl,
                ReportId = reportId
            };

            return Result.Success(response);
        }
        catch (Exception e)
        {
            var error = new Error("500", "Não foi encontrada nenhuma imagem", ErrorType.NotFound);
            logger.LogError(e, "Erro ao listar as imagens: {Error}", error);
            return Result.Failure<ImageListByReportIdResponse>(error);
        }
    }
}