using NewsAPI.Contracts.V1.Model;
using NewsAPI.Contracts.V1;
using NewsAPI.Contracts.V1.Helper;
using System.Threading.Tasks;
using System;

namespace NewsAPI.Business.V1
{
    public interface IRoleHandler
    {
        Task<PaginatedList<RoleResponse>> GetRoleByFilterAsync(RoleQueryFilter filter);
        Task<Response<RoleResponse>> GetRoleByIdAsync(Guid roleId);
        Task<Response<RoleResponse>> CreateRoleAsync(RoleRequest request);
        Task<Response<RoleResponse>> UpdateRoleAsync(Guid roleId, RoleRequest request);
        Task<Response<RoleResponse>> UpdateRoleMenuAsync(Guid roleId, UpdateRoleMenuRequest request);
        Task<Response<RoleResponse>> UpdateRoleAccountAsync(Guid roleId, UpdateRoleAccountRequest request);
        Task<Response<RoleResponse>> DeleteRoleAsync(Guid roleId);
    }
}
