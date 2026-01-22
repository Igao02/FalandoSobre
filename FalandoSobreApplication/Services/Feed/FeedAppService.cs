using FalandoSobre.Domain.Dto.FeedItem;
using FalandoSobre.Domain.Dto.PagedRequest;
using FalandoSobre.Domain.Dto.PagedResponse;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobreApplication.Interfaces.Feed;

namespace FalandoSobreApplication.Services.Feed;

public class FeedAppService : IFeedAppService
{
    //Pra você que está tentando decrifrar, tudo isso foi feito
    //Para aparecer as publicações compartilhadas no feed! Boa sorte =)

    private readonly IReportRepository _reportRepository;
    private readonly ISharedReportRepository _sharedReportRepository;
    private readonly IImageRepository _imageRepository;
    private readonly IUserInfoRepository _userInfoRepository;

    public FeedAppService(
        IReportRepository reportRepository,
        ISharedReportRepository sharedReportRepository,
        IImageRepository imageRepository,
        IUserInfoRepository userInfoRepository)
    {
        _reportRepository = reportRepository;
        _sharedReportRepository = sharedReportRepository;
        _imageRepository = imageRepository;
        _userInfoRepository = userInfoRepository;
    }

    public async Task<PagedResponse<List<FeedItemDTO>>> GetFeedAsync(PagedRequest request)
    {
        // 1️⃣ Busca tudo (sem paginação)
        var reports = await _reportRepository.GetAllAsync();
        var sharedReports = await _sharedReportRepository.GetListAsync();

        // 2️⃣ Dicionário de reports (lookup rápido)
        var reportById = reports.ToDictionary(r => r.Id, r => r);

        // 🔹 IDs de usuários que aparecem no feed
        var userIds = reports
            .Select(r => r.ApplicationUserId)
            .Concat(sharedReports.Select(s => s.ApplicationUserId))
            .Where(id => !string.IsNullOrEmpty(id))
            .Distinct()
            .ToList();

        // 🔹 Busca infos dos usuários
        var usersInfo = await _userInfoRepository.GetAllAsync();

        // 🔹 Lookup rápido por ApplicationUserId
        var userInfoByUserId = usersInfo
            .Where(u => u != null && userIds.Contains(u.ApplicationUserId))
            .ToDictionary(u => u!.ApplicationUserId, u => u!);


        // 3️⃣ Reports normais
        var reportFeedItems = reports.Select(r => new FeedItemDTO
        {
            EventId = r.Id,
            EventDate = r.ReportsDate,
            IsShared = false,
            SharedByUserId = r.ApplicationUserId,
            SharedByUserName = r.UserName,
            Report = r
        });

        // 4️⃣ Shared reports (reconstrói o Report)
        var sharedFeedItems = sharedReports
        .Where(s => s.ReportId != null && reportById.ContainsKey(s.ReportId))
        .Select(s =>
        {
            userInfoByUserId.TryGetValue(s.ApplicationUserId, out var userInfo);

            return new FeedItemDTO
            {
                EventId = s.Id,
                EventDate = s.CreatedAt,
                IsShared = true,
                SharedByUserId = s.ApplicationUserId,
                SharedByUserName = null,
                SharedByUserPhoto = userInfo?.ProfilePhoto, 
                Report = reportById[s.ReportId!]
            };
        });

        // 5️⃣ Feed unificado
        var feed = reportFeedItems
            .Concat(sharedFeedItems)
            .OrderByDescending(f => f.EventDate)
            .ToList();

        // 6️⃣ Paginação FINAL
        var totalItems = feed.Count;

        var pagedFeed = feed
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // 7️⃣ Carrega imagens (somente da página)
        foreach (var item in pagedFeed)
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

        return new PagedResponse<List<FeedItemDTO>>(
            pagedFeed,
            totalItems,
            request.Page,
            request.PageSize
        );
    }
}
