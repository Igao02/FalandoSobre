using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using FalandoSobre.Web.Data;

namespace FalandoSobre.Infrastructure.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly ApplicationDbContext _context;

        public ImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Image?> GetImageAsync(Guid id) => await _context.Images.FindAsync(id);

        public async Task<IEnumerable<Image>> GetListAsync() => await _context.Images.ToListAsync();

        public async Task<Image> AddImageAsync(Image image)
        {
            try
            {
                await _context.AddAsync(image);

                await _context.SaveChangesAsync();

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

            _context.Images.Remove(image!);

           _context.SaveChanges();
        }
    }
}
