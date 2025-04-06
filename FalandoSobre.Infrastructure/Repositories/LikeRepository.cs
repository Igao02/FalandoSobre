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

    public async Task<Like> AddLikesAsync(Like like)
    {
        await _context.AddAsync(like);

        await _context.SaveChangesAsync();

        return like;
    }

    public async Task<IEnumerable<Like>> GetUserLikesAsync(string userName)
    {
        return await _context.Likes
            .Where(l => l.UserName == userName)
            .ToListAsync();
    }

    public async Task RemoveLikesAsync(Guid id)
    {
        var like = await GetAsync(id);

        if (like != null)
        {
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Like?> GetUserLikeAsync(string userName, Guid reportId)
    {
        return await _context.Likes
            .FirstOrDefaultAsync(l => l.UserName == userName && l.ReportId == reportId);
    }

}

