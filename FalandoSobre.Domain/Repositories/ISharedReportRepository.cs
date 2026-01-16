using FalandoSobre.Domain.Entities;

namespace FalandoSobre.Domain.Repositories;

public interface ISharedReportRepository
{
    Task<SharedReport> Create(SharedReport sharedReport);
    Task<IEnumerable<SharedReport>> GetListAsync();
}
