using FalandoSobre.Domain.Dto.FeedItem;
using FalandoSobre.Domain.Dto.PagedRequest;
using FalandoSobre.Domain.Dto.PagedResponse;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobreApplication.Interfaces.Feed;

namespace FalandoSobreApplication.Services.Feed;

public class FeedAppService : IFeedAppService
{
    private readonly IReportRepository _reportRepository;
    private readonly ISharedReportRepository _sharedReportRepository;
    private readonly IImageRepository _imageRepository;

    public FeedAppService(
        IReportRepository reportRepository,
        ISharedReportRepository sharedReportRepository,
        IImageRepository imageRepository)
    {
        _reportRepository = reportRepository;
        _sharedReportRepository = sharedReportRepository;
        _imageRepository = imageRepository;
    }

    public async Task<PagedResponse<List<FeedItemDTO>>> GetFeedAsync(PagedRequest request)
    {
        // 1️ Reports (já paginados)
        var reportsPaged = await _reportRepository.GetListAsync(request);

        var reportFeedItems = reportsPaged.Data
            .Select(r => new FeedItemDTO
            {
                EventId = r.Id,
                EventDate = r.ReportsDate,
                IsShared = false,
                SharedByUserId = null,
                SharedByUserName = null,
                Report = r
            })
            .ToList();

        // 2️ SharedReports (SEM paginação)
        var sharedReports = await _sharedReportRepository.GetListAsync();

        var sharedFeedItems = new List<FeedItemDTO>();

        var reportsById = reportsPaged.Data.ToDictionary(r => r.Id);

        foreach (var shared in sharedReports)
        {
            if (!reportsById.TryGetValue(shared.ReportId, out var report))
                continue;

            sharedFeedItems.Add(new FeedItemDTO
            {
                EventId = shared.Id,
                EventDate = shared.CreatedAt,
                IsShared = true,
                SharedByUserId = shared.ApplicationUserId,
                SharedByUserName = shared.ApplicationUserId,
                Report = report
            });
        }


        // 3️ Unifica e ordena
        var feed = reportFeedItems
            .Concat(sharedFeedItems)
            .OrderByDescending(f => f.EventDate)
            .ToList();

        foreach (var item in feed)
        {
            var images = await _imageRepository.GetImageByReportId(item.Report.Id);

            item.Report.Images = images
                .Select(img => new Image
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl,
                    ReportId = img.ReportId ?? Guid.Empty
                })
                .ToList();
        }

        // 4️ Paginação FINAL do feed
        var totalItems = feed.Count;

        var pagedFeed = feed
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // 5️ Retorno
        return new PagedResponse<List<FeedItemDTO>>(
            pagedFeed,
            totalItems,
            request.Page,
            request.PageSize
        );
    }
}

