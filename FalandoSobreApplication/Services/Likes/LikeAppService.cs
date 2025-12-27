using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobreApplication.Interfaces.Likes;

namespace FalandoSobreApplication.Services.Likes;

public class LikeAppService : ILikeAppService
{
    private readonly ILikeRepository _likeRepository;

    public LikeAppService(ILikeRepository likeRepository)
    {
        _likeRepository = likeRepository;
    }

    public async Task<bool> ToggleLikeAsync(string userId, Guid reportId, bool alreadyLiked)
    {
        if (alreadyLiked)
            return await _likeRepository.RemoveLikeAsync(userId, reportId);

        await _likeRepository.AddLikesAsync(new Like
        {
            ApplicationUserId = userId,
            ReportId = reportId
        });

        return true;
    }

    public async Task<IEnumerable<Guid>> GetLikedReportsByUserAsync(string userId)
    {
        var likes = await _likeRepository.GetLikesByUserIdAsync(userId);
        return likes.Select(l => l.ReportId);
    }

    public async Task<int> GetLikeCountAsync(Guid reportId)
    {
        var likes = await _likeRepository.GetLikesByReportIdAsync(reportId);
        return likes.Count();
    }
}

