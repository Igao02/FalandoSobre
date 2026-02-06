using FalandoSobre.Domain.Dto.FeedItem;
using FalandoSobre.Domain.Dto.PagedRequest;
using FalandoSobre.Domain.Dto.PagedResponse;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobreApplication.Interfaces.Feed;
using System.Text.Json;

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
        // ===============================
        // 1️⃣ Busca dados base
        // ===============================
        var reports = await _reportRepository.GetAllAsync();
        var sharedReports = await _sharedReportRepository.GetListAsync();

        // Lookup rápido de reports
        var reportById = reports.ToDictionary(r => r.Id, r => r);

        // ===============================
        // 2️⃣ Monta FEED SEM resolver usuário
        // ===============================

        // Posts normais
        var reportFeedItems = reports.Select(r => new FeedItemDTO
        {
            EventId = r.Id,
            EventDate = r.ReportsDate,
            IsShared = false,
            SharedByUserId = r.ApplicationUserId,
            SharedByUserName = r.UserName,
            Report = r
        });

        // Posts compartilhados
        var sharedFeedItems = sharedReports
            .Where(s => s.ReportId != null && reportById.ContainsKey(s.ReportId))
            .Select(s => new FeedItemDTO
            {
                EventId = s.Id,
                EventDate = s.CreatedAt,
                IsShared = true,
                SharedByUserId = s.ApplicationUserId,
                SharedByUserName = s.UserName,
                Report = reportById[s.ReportId!]
            });

        // ===============================
        // 3️⃣ Feed unificado + ordenado
        // ===============================
        var feed = reportFeedItems
            .Concat(sharedFeedItems)
            .OrderByDescending(f => f.EventDate)
            .ToList();

        // ===============================
        // 4️⃣ Paginação
        // ===============================
        var totalItems = feed.Count;

        var pagedFeed = feed
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // ===============================
        // 5️⃣ Resolve USUÁRIOS DA PÁGINA
        // ===============================
        var pageUserIds = pagedFeed
            .Select(f => f.SharedByUserId)
            .Where(id => !string.IsNullOrEmpty(id))
            .Distinct()
            .ToList();

        var usersInfo = await _userInfoRepository.GetAllAsync();

        var userInfoByUserId = usersInfo
            .Where(u => u != null && pageUserIds.Contains(u.ApplicationUserId))
            .ToDictionary(u => u!.ApplicationUserId, u => u!);

        // Preenche nome + foto
        foreach (var item in pagedFeed)
        {
            if (item.SharedByUserId != null &&
                userInfoByUserId.TryGetValue(item.SharedByUserId, out var userInfo))
            {
                item.SharedByUserPhoto = userInfo.ProfilePhoto;
            }
        }

        // ===============================
        // 6️⃣ Carrega imagens dos reports
        // ===============================
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

        // ===============================
        // 7️⃣ Retorno final
        // ===============================
        return new PagedResponse<List<FeedItemDTO>>(
            pagedFeed,
            totalItems,
            request.Page,
            request.PageSize
        );
    }

}
