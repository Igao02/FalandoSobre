using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace FalandoSobre.Infrastructure.Repositories;

public class SharedReportRepository : ISharedReportRepository
{
    private readonly ApplicationDbContext _context;

    public SharedReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SharedReport> Create(SharedReport sharedReport)
    {
        await _context.AddAsync(sharedReport);
        await _context.SaveChangesAsync();
        return sharedReport;
    }

    public async Task<IEnumerable<SharedReport>> GetListAsync()
    {
        return await _context.SharedReports
            .Include(s => s.Report)
            .ToListAsync();
    }
}
