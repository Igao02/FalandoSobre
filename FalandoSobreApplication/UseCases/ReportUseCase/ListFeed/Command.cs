using Application.Abstractions.Messaging;

namespace FalandoSobreApplication.UseCases.ReportUseCase.ListFeed;

public sealed record ListFeedCommand() : ICommand<List<ListFeedResponse>>;
