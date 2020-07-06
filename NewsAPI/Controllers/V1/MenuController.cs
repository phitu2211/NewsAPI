using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewsAPI.Contracts.V1.Helper;
using NewsAPI.Contracts.V1.Model;
using Newtonsoft.Json;
using NewsAPI.Business.V1;
using System.Collections.Generic;

namespace NewsAPI.Controllers.V1
{
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuHandler _menuService;

        public MenuController(IMenuHandler menuService)
        {
            _menuService = menuService;
        }

        [HttpGet(Constant.ApiRoutes.Menu.GetMenus)]
        public async Task<PaginatedList<MenuResponse>> GetByFilterAsync(string filter = "{}")
        {
            var filterConvert = JsonConvert.DeserializeObject<MenuQueryFilter>(filter);
            return await _menuService.GetByFilterAsync(filterConvert);
        }

        [HttpPost(Constant.ApiRoutes.Menu.CreateMenu)]
        public async Task<Response<MenuResponse>> CreateMenu([FromBody] MenuRequest request)
        {
            return await _menuService.CreateMenuAsync(request);
        }

        [HttpPost(Constant.ApiRoutes.Menu.GetMenusByRoleName)]
        public async Task<Response<List<MenuResponse>>> GetMenusByRoleNames([FromBody] GetMenusByRoleNameRequest roleName)
        {
            return await _menuService.GetMenusByRoleNamesAsync(roleName);
        }

        [HttpPut(Constant.ApiRoutes.Menu.UpdateMenu)]
        public async Task<Response<MenuResponse>> UpdateMenu([FromRoute] Guid menuId, [FromBody] MenuUpdateRequest request)
        {
            return await _menuService.UpdateMenuAsync(menuId, request);
        }

        [HttpDelete(Constant.ApiRoutes.Menu.DeleteMenu)]
        public async Task<Response<MenuResponse>> DeleteMenu([FromRoute] Guid menuId)
        {
            return await _menuService.DeleteMenuAsync(menuId);
        }

        [HttpGet(Constant.ApiRoutes.Menu.GetMenuById)]
        public async Task<Response<MenuResponse>> GetMenuById([FromRoute] Guid menuId)
        {
            return await _menuService.GetMenuByIdAsync(menuId);
        }
    
        [HttpGet(Constant.ApiRoutes.Menu.GetMenusByRoleId)]
        public async Task<Response<GetMenuByRoleIdResponse>> GetMenusByRoleId([FromRoute] string roleId)
        {
            return await _menuService.GetMenusByRoleIdAsync(roleId);
        }

        [HttpGet(Constant.ApiRoutes.Menu.GetSubMenusById)]
        public async Task<Response<List<MenuResponse>>> GetSubMenusById([FromRoute] Guid menuId)
        {
            return await _menuService.GetSubMenusByIdAsync(menuId);
        }
    }
}