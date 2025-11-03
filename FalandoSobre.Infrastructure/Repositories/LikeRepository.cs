using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace FalandoSobre.Infrastructure.Repositories;

public class LikeRepository : ILikeRepository
{
    private readonly ApplicationDbContext _context;

    public LikeRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Like?> GetAsync(Guid id) => await _context.Likes.FindAsync(id);

    public async Task<IEnumerable<Like>> GetLikesAsync() => await _context.Likes.ToListAsync();

    public async Task<IEnumerable<Like>> GetLikesByUserIdAsync(string userId) =>
        await _context.Likes.Where(l => l.ApplicationUserId == userId).ToListAsync();

    public async Task<IEnumerable<Like>> GetLikesByReportIdAsync(Guid reportId) =>
        await _context.Likes.Where(l => l.ReportId == reportId && l.Actived).ToListAsync();

    public async Task<Like?> GetLikeByUserAndReportAsync(string userId, Guid reportId) =>
        await _context.Likes.FirstOrDefaultAsync(l => l.ApplicationUserId == userId && l.ReportId == reportId && l.Actived);

    public async Task<Like> AddLikesAsync(Like like)
    {
        await _context.AddAsync(like);

        await _context.SaveChangesAsync();

        return like;
    }

    public async Task<bool> RemoveLikeAsync(string userId, Guid reportId)
    {
        var like = await GetLikeByUserAndReportAsync(userId, reportId);

        if (like != null)
        {
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}

