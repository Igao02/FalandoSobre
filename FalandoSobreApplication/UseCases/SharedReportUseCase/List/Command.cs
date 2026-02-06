using Application.Abstractions.Messaging;

namespace FalandoSobreApplication.UseCases.SharedReportUseCase.List;

public sealed record ListSharedReportCommand() : ICommand<List<ListSharedReportResponse>>;
