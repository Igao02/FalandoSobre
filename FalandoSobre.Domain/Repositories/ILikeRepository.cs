using FalandoSobre.Domain.Entities;

namespace FalandoSobre.Domain.Repositories;

public interface ILikeRepository
{
    Task<Like> AddLikesAsync(Like like);
    Task<Like?> GetAsync(Guid id);
    Task<IEnumerable<Like>> GetLikesAsync();
    Task<IEnumerable<Like>> GetLikesByUserIdAsync(string userId);
    Task<IEnumerable<Like>> GetLikesByReportIdAsync(Guid reportId);
    Task<Like?> GetLikeByUserAndReportAsync(string userId, Guid reportId);
    Task<bool> RemoveLikeAsync(string userId, Guid reportId);
}
