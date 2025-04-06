using FalandoSobre.Domain.Entities;

namespace FalandoSobre.Domain.Repositories;

public interface ILikeRepository
{
    Task<Like> AddLikesAsync(Like like);
    Task<Like?> GetAsync(Guid id);
    Task<IEnumerable<Like>> GetLikesAsync();
    Task<Like?> GetUserLikeAsync(string userName, Guid reportId);
    Task<IEnumerable<Like>> GetUserLikesAsync(string userName);
    Task RemoveLikesAsync(Guid id);
}
