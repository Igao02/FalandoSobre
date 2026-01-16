using FalandoSobre.Domain.Entities;

namespace FalandoSobreApplication.Interfaces.SharedReports;

public interface ISharedReportsAppService
{
    Task<SharedReport> AddAsync(Guid reportId);
}
