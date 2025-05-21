using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.Web.Data;

namespace FalandoSobre.Infrastructure.Repositories;

public class LogRepository : ILogRepository
{
    private readonly ApplicationDbContext _context;

    public LogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Logs> Create(Logs logs)
    {
        await _context.AddAsync(logs);
        await _context.SaveChangesAsync();
        return logs;
    }
}
