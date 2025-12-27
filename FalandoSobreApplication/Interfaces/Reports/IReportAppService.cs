using FalandoSobre.Domain.Entities;

namespace FalandoSobreApplication.Interfaces.Reports;

public interface IReportAppService
{
    Task<(List<Report> Reports, int TotalItems)> GetReportsAsync(int page, int pageSize);
    Task<List<UserInfo>> GetProfilePhotosAsync(List<Report> reports);
}
