using FuelManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FuelManagementAPI.Repositories.IRepositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByMobileAsync(string mobile);
        Task<bool> MobileExistsAsync(string mobile);
        Task<IEnumerable<User>> GetAllAsync();
      //  Task<bool> EmailExistsAsync(string email);
        Task<User> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);

    }
}
