using FalandoSobre.Domain.Dto.PagedRequest;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobreApplication.Interfaces;

namespace FalandoSobreApplication.Services;

public class ReportAppService(IReportRepository reportRepo, IImageRepository imageRepo, IUserInfoRepository userRepo) : IReportAppService
{
    private readonly IReportRepository _reportRepository = reportRepo;
    private readonly IImageRepository _imageRepository = imageRepo;
    private readonly IUserInfoRepository _userInfoRepository = userRepo;

   

    public async Task<List<UserInfo>> GetProfilePhotosAsync(List<Report> reports)
    {
        var result = new List<UserInfo>();

        foreach (var pub in reports)
        {
            var imageResult = await _userInfoRepository.GetImageByUserId(pub.ApplicationUserId);

            result.Add(imageResult ?? new UserInfo
            {
                Id = Guid.Empty,
                ProfilePhoto = string.Empty,
                ApplicationUserId = pub.ApplicationUserId,
                CreatedAt = DateTime.UtcNow
            });
        }

        return result;
    }

    public async Task<(List<Report> Reports, int TotalItems)> GetReportsAsync(int page, int pageSize)
    {
        var pagedRequest = new PagedRequest { Page = page, PageSize = pageSize };
        var pagedResult = await _reportRepository.GetListAsync(pagedRequest);

        foreach (var report in pagedResult.Data)
        {
            var imageResults = await _imageRepository.GetImageByReportId(report.Id);

            report.Images = imageResults
                .Select(img => new Image
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl,
                    ReportId = img.ReportId ?? Guid.Empty
                })
                .ToList();
        }

        return (pagedResult.Data, pagedResult.TotalItems);
    }

}
