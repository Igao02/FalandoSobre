using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.Web.Data;

namespace FalandoSobre.Infrastructure.Repositories;

public class UserInfoRepository : IUserInfoRepository
{
    private readonly ApplicationDbContext _context;
    public UserInfoRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<UserInfo> AddAsync(UserInfo userInfo)
    {
        await _context.UserInfos.AddAsync(userInfo);
        await _context.SaveChangesAsync();
        return userInfo;
    }
    public async Task<UserInfo> Save(UserInfo userInfo)
    {
        _context.UserInfos.Update(userInfo);
        await _context.SaveChangesAsync();
        return userInfo;
    }
}
