using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Dto.PagedResponse;

namespace FalandoSobreApplication.UseCases.ReportUseCase.ListUseCase;

public sealed record ListReportCommand(int Page, int PageSize)
    : ICommand<PagedResponse<ListReportReponse[]>>;
