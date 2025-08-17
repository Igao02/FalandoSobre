using FalandoSobre.Domain.Entities;

namespace FalandoSobre.Domain.Repositories;

public interface IImageRepository
{
    Task<Image> AddImageAsync(Image image);
    Task DeleteImageAsync(Guid id);
    Task <Image?> GetImageAsync(Guid id);
    Task<IEnumerable<Image>> GetListAsync();
    Task<IEnumerable<(Guid Id, string ImageUrl, Guid? ReportId)>> GetImageByReportId(Guid id);
}
