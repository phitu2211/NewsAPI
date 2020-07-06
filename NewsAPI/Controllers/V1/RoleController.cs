using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewsAPI.Contracts.V1.Helper;
using NewsAPI.Contracts.V1.Model;
using Newtonsoft.Json;
using NewsAPI.Business.V1;
using System;

namespace NewsAPI.Controllers.V1
{
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleHandler _roleService;

        public RoleController(IRoleHandler roleService)
        {
            _roleService = roleService;
        }

        [HttpGet(Constant.ApiRoutes.Role.GetRoleById)]
        public async Task<Response<RoleResponse>> GetRoleById([FromRoute] Guid roleId)
        {
            return await _roleService.GetRoleByIdAsync(roleId);
        }

        [HttpGet(Constant.ApiRoutes.Role.GetRoleByFilter)]
        public async Task<PaginatedList<RoleResponse>> GetRoleByFilter(string filter = "{}")
        {
            var filterConvert = JsonConvert.DeserializeObject<RoleQueryFilter>(filter);

            return await _roleService.GetRoleByFilterAsync(filterConvert);
        }

        [HttpPost(Constant.ApiRoutes.Role.CreateRole)]
        public async Task<Response<RoleResponse>> CreateRole([FromBody] RoleRequest request)
        {
            return await _roleService.CreateRoleAsync(request);
        }

        [HttpDelete(Constant.ApiRoutes.Role.DeleteRole)]
        public async Task<Response<RoleResponse>> DeleteRole([FromRoute] Guid roleId)
        {
            return await _roleService.DeleteRoleAsync(roleId);
        }

        [HttpPut(Constant.ApiRoutes.Role.UpdateRole)]
        public async Task<Response<RoleResponse>> UpdateRole([FromRoute] Guid roleId, [FromBody] RoleRequest request)
        {
            return await _roleService.UpdateRoleAsync(roleId, request);
        }

        [HttpPut(Constant.ApiRoutes.Role.UpdateRoleAccount)]
        public async Task<Response<RoleResponse>> UpdateRoleAccount([FromRoute] Guid roleId, [FromBody] UpdateRoleAccountRequest request)
        {
            return await _roleService.UpdateRoleAccountAsync(roleId, request);
        }

        [HttpPut(Constant.ApiRoutes.Role.UpdateRoleMenu)]
        public async Task<Response<RoleResponse>> UpdateRoleMenu([FromRoute] Guid roleId, [FromBody] UpdateRoleMenuRequest request)
        {
            return await _roleService.UpdateRoleMenuAsync(roleId, request);
        }
    }
}