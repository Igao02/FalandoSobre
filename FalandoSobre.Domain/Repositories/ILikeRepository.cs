using FalandoSobre.Domain.Entities;

namespace FalandoSobre.Domain.Repositories;

public interface ILikeRepository
{
    Task<Like> AddLikesAsync(Like like);
    Task<Like?> GetAsync(Guid id);
    Task<IEnumerable<Like>> GetLikesAsync();
    Task RemoveLikesAsync(Guid id);
}
