using BlazorAdmin.Data.Models;

namespace BlazorAdmin.Data.Services
{
    public interface IRoleService
    {
        Task<List<Role>> GetRoles();
        Task<List<Role>> GetRolesUser(Guid id);
        Task<bool> CreateRole(Role role);
        Task<Role> GetRole(Guid id);
        Task<bool> EditRole(Guid id, Role role);
        Task<bool> DeleteRole(Guid id);
    }
}