using NewsAPI.Contracts.V1.Model;
using NewsAPI.Contracts.V1;
using NewsAPI.Contracts.V1.Helper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsAPI.Business.V1
{
    public interface IMenuHandler
    {
        Task<PaginatedList<MenuResponse>> GetByFilterAsync(MenuQueryFilter filter);
        Task<Response<MenuResponse>> GetMenuByIdAsync(Guid menuId);
        Task<Response<List<MenuResponse>>> GetMenusByRoleNamesAsync(GetMenusByRoleNameRequest request);
        Task<Response<GetMenuByRoleIdResponse>> GetMenusByRoleIdAsync(string roleId);
        Task<Response<MenuResponse>> CreateMenuAsync(MenuRequest request);
        Task<Response<MenuResponse>> UpdateMenuAsync(Guid menuId, MenuUpdateRequest request);
        Task<Response<MenuResponse>> DeleteMenuAsync(Guid menuId);
        Task<Response<List<MenuResponse>>> GetSubMenusByIdAsync(Guid menuId);
    }
}
