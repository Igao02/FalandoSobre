namespace FalandoSobreApplication.Interfaces.Likes;

public interface ILikeAppService
{
    Task<bool> ToggleLikeAsync(string userId, Guid reportId, bool alreadyLiked);
    Task<IEnumerable<Guid>> GetLikedReportsByUserAsync(string userId);
    Task<int> GetLikeCountAsync(Guid reportId);
}

