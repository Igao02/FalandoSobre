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

    public async Task<IEnumerable<Report>> GetListAsync() => await _context.Reports.OrderByDescending(r => r.ReportsDate).ToListAsync();

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
