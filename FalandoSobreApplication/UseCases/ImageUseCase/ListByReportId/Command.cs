using Application.Abstractions.Messaging;

namespace FalandoSobreApplication.UseCases.ImageUseCase.ListByReportId;

public sealed record ImageListByReportIdCommand(Guid Id) : ICommand<ImageListByReportIdResponse>;