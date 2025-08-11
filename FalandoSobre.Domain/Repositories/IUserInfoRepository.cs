using FalandoSobre.Domain.Entities;

namespace FalandoSobre.Domain.Repositories;

public interface IUserInfoRepository
{
    Task<UserInfo> AddAsync(UserInfo userInfo);
    Task<IEnumerable<UserInfo?>> GetAllAsync();
    Task<UserInfo?> GetImageByUserId(string userId);
    Task<UserInfo> Save(UserInfo userInfo);
}
