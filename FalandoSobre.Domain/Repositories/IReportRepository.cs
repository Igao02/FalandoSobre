using FalandoSobre.Domain.Dto.PagedRequest;
using FalandoSobre.Domain.Dto.PagedResponse;
using FalandoSobre.Domain.Entities;

namespace FalandoSobre.Domain.Repositories;

public interface IReportRepository
{
    Task<Report> AddAsync(Report report);
    Task DeleteAsync(Guid id);
    Task<Report> EditAsync(Report report);
    Task<Report?> GetAsync(Guid id);
    //Task<IEnumerable<Report>> GetListAsync();
    Task<PagedResponse<List<Report>>> GetListAsync(PagedRequest request);

    Task<IEnumerable<Report>> GetReportsByTypeAsync(string type);
}