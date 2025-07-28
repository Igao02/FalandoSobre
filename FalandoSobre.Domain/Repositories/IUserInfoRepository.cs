using FalandoSobre.Domain.Entities;

namespace FalandoSobre.Domain.Repositories;

public interface IUserInfoRepository
{
    Task<UserInfo> AddAsync(UserInfo userInfo);
    Task<UserInfo> Save(UserInfo userInfo);
}
