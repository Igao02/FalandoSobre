using FalandoSobre.Domain.Dto.FeedItem;
using FalandoSobre.Domain.Dto.PagedRequest;
using FalandoSobre.Domain.Dto.PagedResponse;

namespace FalandoSobreApplication.Interfaces.Feed;

public interface IFeedAppService
{
    Task<PagedResponse<List<FeedItemDTO>>> GetFeedAsync(PagedRequest request); 
}
