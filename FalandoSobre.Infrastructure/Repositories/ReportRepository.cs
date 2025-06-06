using FalandoSobre.Domain.Dto.PagedRequest;
using FalandoSobre.Domain.Dto.PagedResponse;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace FalandoSobre.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _context;

    public ReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<List<Report>>> GetListAsync(PagedRequest request)
    {
        var query = _context.Reports.OrderByDescending(r => r.ReportsDate).AsQueryable();

        var totalItems = await query.CountAsync();

        var data = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResponse<List<Report>>(data, totalItems, request.Page, request.PageSize);
    }

    public async Task<IEnumerable<Report>> GetReportsByTypeAsync(string type)
    {
        return await _context.Reports
            .Where(r => r.TypeReport == type)
            .OrderByDescending(r => r.ReportsDate)
            .ToListAsync();
    }

    public async Task<Report?> GetAsync(Guid id) => await _context.Reports.FindAsync(id);

    public async Task<Report> AddAsync(Report report)
    {
        await _context.AddAsync(report);
        await _context.SaveChangesAsync();
        return report;
    }

    public async Task DeleteAsync(Guid id)
    {
        var report = await GetAsync(id);

        _context.Reports.Remove(report!);

        await _context.SaveChangesAsync();
    }

    public async Task<Report> EditAsync(Report report)
    {
        _context.Entry(report).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return report;
    }

}
