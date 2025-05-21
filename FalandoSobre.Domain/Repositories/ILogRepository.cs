using FalandoSobre.Domain.Entities;

namespace FalandoSobre.Domain.Repositories;

public interface ILogRepository
{
    Task<Logs> Create(Logs logs);
}
