using Application.Abstractions.Messaging;
using FalandoSobre.Domain.Entities;

namespace FalandoSobre.Application.UseCases.ReportUseCase.CreateUseCase;

public sealed record CreateReportCommand(string ReportName, string TypeReport, string ReportDescription, string UserName, bool IsEvent)
    : ICommand<Report>;
