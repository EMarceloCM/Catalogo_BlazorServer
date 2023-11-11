using BlazorAdmin.Data.Models;

namespace BlazorAdmin.Data.Services
{
    public interface IUserService
    {
        Task<List<User>> GetUsers();
        Task<User> GetUser(Guid id);
        Task<bool> DeleteUser(Guid id);
        Task<bool> UpdateUserRole(Guid id, User user);
    }
}
