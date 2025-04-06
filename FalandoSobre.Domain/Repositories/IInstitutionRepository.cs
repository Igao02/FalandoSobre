namespace FalandoSobre.Domain.Repositories;

public interface IInstitutionRepository
{
    Task<Institution> AddAsync(Institution institution);
    Task DeleteAsync(Guid id);
    Task<Institution> EditAsync(Institution institution);
    Task<Institution?> GetAsync(Guid id);
    Task<Institution?> GetByDocAsync(string doc);
    Task<Institution?> GetByNameAsync(string name);
    Task<IEnumerable<Institution>> GetListAsync();
}
