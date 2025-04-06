using FalandoSobre.Domain.Entities;

namespace FalandoSobre.Domain.Repositories;

public interface ICommentRepository
{
    Task<Comment> AddAsync(Comment comment);
    Task DeleteAsync(Guid id);
    Task<Comment> EditAsync(Comment comment);
    Task<IEnumerable<Comment>> GetListAsync();
    Task<Comment?> GetAsync(Guid id);
    Task<int> SumCommentAsync(Guid id);
}
