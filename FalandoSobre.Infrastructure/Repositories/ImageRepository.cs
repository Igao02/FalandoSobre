using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using FalandoSobre.Web.Data;

namespace FalandoSobre.Infrastructure.Repositories
{
    public class ImageRepository(ApplicationDbContext context) : IImageRepository
    {
        public async Task<Image> AddImageAsync(Image image)
        {
            try
            {
                await context.AddAsync(image);

                await context.SaveChangesAsync();

                return image;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Erro (repositório infra) ao adicionar imagem: {ex.Message}");
                throw new ArgumentException($"Stack Trace: {ex.StackTrace}");

            }
        }
        public async Task DeleteImageAsync(Guid id)
        {
            var image = await GetImageAsync(id);

            context.Images.Remove(image!);

           context.SaveChanges();
        }

        public async Task<Image?> GetImageAsync(Guid id) => await context.Images.FindAsync(id);
        public async Task<IEnumerable<Image>> GetListAsync() => await context.Images.ToListAsync();
        public async Task<(Guid Id, string ImageUrl, Guid? ReportId)?> GetImageByReportId(Guid id)
        {
            return await context.Images
                .Where(i => i.ReportId == id)
                .Select(i => new ValueTuple<Guid, string, Guid?>(
                    i.Id,
                    i.ImageUrl,
                    i.ReportId
                ))
                .FirstOrDefaultAsync();
        }

    }
}
