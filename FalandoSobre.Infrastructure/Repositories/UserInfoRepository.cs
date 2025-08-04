using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.Web.Data;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IEnumerable<UserInfo?>> GetAllAsync()
    {
        return await _context.UserInfos
            .Where(u => u.Actived == true)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

    }

    public async Task<UserInfo?> GetImageByUserId(string userId)
    {
       return await _context.UserInfos
            .Where(u => u.ApplicationUserId == userId)
            .Select(u => new UserInfo
            {
                Id = u.Id,
                ProfilePhoto = u.ProfilePhoto,
                ApplicationUserId = u.ApplicationUserId
            })
            .FirstOrDefaultAsync();
    }

    public async Task<UserInfo> Save(UserInfo userInfo)
    {
        _context.UserInfos.Update(userInfo);
        await _context.SaveChangesAsync();
        return userInfo;
    }
}
