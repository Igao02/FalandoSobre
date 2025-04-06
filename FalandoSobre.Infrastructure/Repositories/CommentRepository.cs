using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace FalandoSobre.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly ApplicationDbContext _context;

    public CommentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Comment?> GetAsync(Guid id) => await _context.Comments.FindAsync(id);

    public async Task<IEnumerable<Comment>> GetListAsync() => await _context.Comments.ToListAsync();

    public async Task<Comment> AddAsync(Comment comment)
    {
        await _context.AddAsync(comment);

        await _context.SaveChangesAsync();

        return comment;
    }

    public async Task DeleteAsync(Guid id)
    {
        var comment = await GetAsync(id);

        _context.Comments.Remove(comment!);
    }

    public async Task<Comment> EditAsync(Comment comment)
    {
        _context.Entry(comment).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return comment;
    }

    public async Task<int> SumCommentAsync(Guid id)
    {
        return await _context.Comments.CountAsync(c => c.Id == id);
    }
}

