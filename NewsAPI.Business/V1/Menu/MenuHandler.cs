using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAPI.Contracts.V1;
using NewsAPI.Contracts.V1.Helper;
using NewsAPI.Contracts.V1.Model;
using NewsAPI.Data;
using NewsAPI.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsAPI.Business.V1
{
    public class MenuHandler : IMenuHandler
    {
        private readonly NewsContext _newsContext;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ILogger<MenuHandler> _logger;

        public MenuHandler(NewsContext newsContext, RoleManager<AppRole> roleManager, ILogger<MenuHandler> logger)
        {
            _logger = logger;
            _roleManager = roleManager;
            _newsContext = newsContext;
        }

        #region CRUD
        /// <summary>
        /// Tạo mới menu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Response<MenuResponse>> CreateMenuAsync(MenuRequest request)
        {
            if (string.IsNullOrEmpty(request.MenuName) || request.Role.Count < 1 || request.Role == null)
            {
                _logger.LogError("Name menu not null, menu have to role");
                return new Response<MenuResponse>(Constant.STATUS_ERROR, new List<string> { "Name menu is not null", "Menu have to role" });
            }
            var menu = new Menu
            {
                Id = Guid.NewGuid(),
                MenuName = request.MenuName
            };

            if (request.ParentId == null)
                menu.ParentId = null;
            else
                menu.ParentId = request.ParentId;

            var menuRole = new List<MenuRole>();

            //set quyền
            foreach (var item in request.Role)
            {
                var role = await _roleManager.FindByNameAsync(item);
                if (role == null)
                {
                    _logger.LogError("Not find role");
                    return new Response<MenuResponse>(Constant.STATUS_ERROR, new List<string> { "Not find role" });
                }
                menuRole.Add(new MenuRole { MenuId = menu.Id, Id = Guid.NewGuid(), RoleId = role.Id });
            }

            await _newsContext.MenuRoles.AddRangeAsync(menuRole);

            await _newsContext.Menus.AddAsync(menu);

            var result = await _newsContext.SaveChangesAsync();

            var dataResponse = new MenuResponse
            {
                Id = menu.Id,
                Role = await GetRolesAsync(menu.Id),
                MenuName = menu.MenuName,
                ParentId = menu.ParentId,
                SubMenus = null
            };

            if (result > 0)
            {
                _logger.LogInformation("Create menu success");
                return new Response<MenuResponse>(Constant.STATUS_SUCESS, null, dataResponse, 1);
            }

            _logger.LogError("Create menu failed");
            return new Response<MenuResponse>(Constant.STATUS_ERROR, new List<string> { "Error when save" });
        }

        /// <summary>
        /// Xóa menu
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public async Task<Response<MenuResponse>> DeleteMenuAsync(Guid menuId)
        {
            var menu = await _newsContext.Menus.FindAsync(menuId);

            if (menu == null)
            {
                _logger.LogError("Not find menu");
                return new Response<MenuResponse>(Constant.STATUS_ERROR, new List<string> { "Not find menu" });
            }

            if (menu.ParentId == null)
            {
                var subMenus = _newsContext.Menus.Where(x => x.ParentId == menu.Id);

                //Tìm xóa menu con nếu có
                foreach (var item in subMenus)
                {
                    var roleMenus = _newsContext.MenuRoles.Where(x => x.MenuId == item.Id);

                    if (roleMenus.Count() > 0 || roleMenus != null)
                        _newsContext.MenuRoles.RemoveRange(roleMenus);
                }

                if (subMenus != null || subMenus.Count() > 0)
                    _newsContext.Menus.RemoveRange(subMenus);
            }

            var roleMenu = _newsContext.MenuRoles.Where(x => x.MenuId == menuId);

            if (roleMenu.Count() > 0 || roleMenu != null)
                _newsContext.MenuRoles.RemoveRange(roleMenu);

            _newsContext.Menus.Remove(menu);

            var result = await _newsContext.SaveChangesAsync();

            if (result > 0)
            {
                _logger.LogInformation("Delete menu success");
                return new Response<MenuResponse>(Constant.STATUS_SUCESS);
            }
            _logger.LogError("Delete menu failed");

            return new Response<MenuResponse>(Constant.STATUS_ERROR, new List<string> { "Error when save" });
        }

        /// <summary>
        /// Lấy danh sách menu theo filter: tên,..
        /// phân trang
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<PaginatedList<MenuResponse>> GetByFilterAsync(MenuQueryFilter filter)
        {
            var data = _newsContext.Menus.Where(x => x.ParentId == null);

            var menuRes = new List<MenuResponse>();

            foreach (var menu in data)
            {
                menuRes.Add(new MenuResponse
                {
                    Id = menu.Id,
                    MenuName = menu.MenuName,
                    ParentId = menu.ParentId,
                    SubMenus = await GetSubMenusAsync(menu.Id),
                    Role = await GetRolesAsync(menu.Id)
                });
            }

            if (!string.IsNullOrEmpty(filter.MenuName))
                menuRes = menuRes.Where(x => x.MenuName == filter.MenuName).ToList();

            if (!filter.PageSize.HasValue && !filter.PageNumber.HasValue)
                return await PaginatedList<MenuResponse>.CreateAsync(menuRes);

            return await PaginatedList<MenuResponse>.CreateAsync(menuRes, filter.PageNumber.Value, filter.PageSize.Value);
        }

        /// <summary>
        /// xem chi tiết menu
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public async Task<Response<MenuResponse>> GetMenuByIdAsync(Guid menuId)
        {
            var menu = await _newsContext.Menus.FindAsync(menuId);

            if (menu == null)
            {
                _logger.LogError("Not find menu");
                return new Response<MenuResponse>(Constant.STATUS_ERROR, new List<string> { "Not find menu" });
            }

            var dataResponse = new MenuResponse
            {
                Id = menu.Id,
                MenuName = menu.MenuName,
                ParentId = menu.ParentId,
                Role = await GetRolesAsync(menu.Id),
                SubMenus = await GetSubMenusAsync(menu.Id)
            };

            return new Response<MenuResponse>(Constant.STATUS_SUCESS, null, dataResponse, 1);
        }

        /// <summary>
        /// cập nhật menu, thêm xóa quyền
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Response<MenuResponse>> UpdateMenuAsync(Guid menuId, MenuUpdateRequest request)
        {
            var menu = await _newsContext.Menus.FindAsync(menuId);

            if (menu == null)
            {
                _logger.LogError("Not find menu");
                return new Response<MenuResponse>(Constant.STATUS_ERROR, new List<string> { "Not find menu" });
            }

            if (!string.IsNullOrEmpty(request.MenuName))
                menu.MenuName = request.MenuName;

            menu.ParentId = request.ParentId ?? null;

            //Thêm quyền vào menu
            foreach (var item in request.AddRoles ?? new List<string> { })
            {
                var role = await _roleManager.FindByNameAsync(item);
                if (role != null)
                {
                    var menuRole = _newsContext.MenuRoles.Where(x => x.MenuId.Equals(menu.Id) && x.RoleId.Equals(role.Id));
                    if (menuRole != null && menuRole.Any())
                    {
                        _logger.LogError("Role already in menu");
                        return new Response<MenuResponse>(Constant.STATUS_ERROR, new List<string> { "Role already in menu" });
                    }
                    await _newsContext.MenuRoles.AddAsync(new MenuRole
                    {
                        Id = Guid.NewGuid(),
                        MenuId = menu.Id,
                        RoleId = role.Id
                    });
                }
            }

            //Xóa quyền khỏi menu
            foreach (var item in request.RemoveRoles ?? new List<string> { })
            {
                var role = await _roleManager.FindByNameAsync(item);
                if (role != null)
                {
                    var rm = _newsContext.MenuRoles.Where(x => x.RoleId == role.Id && x.MenuId == menu.Id);

                    _newsContext.MenuRoles.RemoveRange(rm);
                }
            }

            var result = await _newsContext.SaveChangesAsync();

            var dataResponse = new MenuResponse
            {
                Id = menu.Id,
                MenuName = menu.MenuName,
                ParentId = menu.ParentId,
                SubMenus = await GetSubMenusAsync(menu.Id),
                Role = await GetRolesAsync(menu.Id)
            };

            if (result > 0)
            {
                _logger.LogInformation("Update menu success");
                return new Response<MenuResponse>(Constant.STATUS_SUCESS, null, dataResponse, 1);
            }

            _logger.LogError("Update menu error");
            return new Response<MenuResponse>(Constant.STATUS_ERROR, new List<string> { "Error when save" });
        }
        #endregion

        /// <summary>
        /// Lấy danh sách menu theo tên quyền
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Response<List<MenuResponse>>> GetMenusByRoleNamesAsync(GetMenusByRoleNameRequest request)
        {
            if (request.RoleNames.Count() < 0 || request.RoleNames == null)
            {
                _logger.LogError("No role input");
                return new Response<List<MenuResponse>>(Constant.STATUS_ERROR, new List<string> { "No role input" });
            }

            var listRoleId = new List<Guid>();

            foreach (var item in request.RoleNames)
            {
                var roleId = (await _roleManager.FindByNameAsync(item)).Id;
                if (roleId != null)
                    listRoleId.Add(roleId);
            }

            var menuRoles = new List<MenuRole>();

            foreach (var roleId in listRoleId)
            {
                menuRoles.AddRange(_newsContext.MenuRoles.Where(x => x.RoleId.Equals(roleId)));
            }

            if (menuRoles == null || menuRoles.Count() < 1)
            {
                _logger.LogError("No menu is accessed");
                return new Response<List<MenuResponse>>(Constant.STATUS_SUCESS, new List<string> { "No menu is accessed" });
            }
            var menuResponses = new List<MenuResponse>();

            foreach (var item in menuRoles)
            {
                var menu = await _newsContext.Menus.FindAsync(item.MenuId);

                if(menu.ParentId == null)
                    menuResponses.Add(new MenuResponse
                    {
                        Id = menu.Id,
                        MenuName = menu.MenuName,
                        ParentId = menu.ParentId,
                        SubMenus = await GetSubMenusAsync(menu.Id),
                        Role = await GetRolesAsync(menu.Id)
                    });
            }

            return new Response<List<MenuResponse>>(Constant.STATUS_SUCESS, null, menuResponses, menuResponses.Count());
        }

        /// <summary>
        /// Lấy danh sách menu theo id quyền
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<Response<GetMenuByRoleIdResponse>> GetMenusByRoleIdAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                _logger.LogError("No find role with id");
                return new Response<GetMenuByRoleIdResponse>(Constant.STATUS_ERROR, new List<string> { "No find role with id" });
            }

            var dataResponse = new GetMenuByRoleIdResponse();
            //ds menu có quyền
            var menuHaveRole = new List<MenuResponse>();
            //ds menu k có quyền
            var menuNoRole = new List<MenuResponse>();

            //thêm menu vào 2 ds trên
            foreach (var item in _newsContext.Menus)
            {
                var list = IsMenuInRole(item, role) ? menuHaveRole : menuNoRole;
                list.Add(new MenuResponse
                {
                    Id = item.Id,
                    MenuName = item.MenuName,
                    ParentId = item.ParentId,
                    SubMenus = await GetSubMenusAsync(item.Id),
                    Role = await GetRolesAsync(item.Id)
                });
            }

            dataResponse.Role = menuHaveRole;
            dataResponse.NoRole = menuNoRole;

            return new Response<GetMenuByRoleIdResponse>(Constant.STATUS_SUCESS, null, dataResponse, 1);
        }

        /// <summary>
        /// Lấy danh sách menu con của menu cha
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public async Task<Response<List<MenuResponse>>> GetSubMenusByIdAsync(Guid menuId)
        {
            if (string.IsNullOrEmpty(menuId.ToString()))
            {
                _logger.LogError("Id not null");
                return new Response<List<MenuResponse>>(Constant.STATUS_ERROR, new List<string> { "Id not null" });
            }

            var subs = await GetSubMenusAsync(menuId);
            var dataResponse = new List<MenuResponse>();

            if(subs != null)
                dataResponse = (await Task.WhenAll(subs.Select(async x => new MenuResponse
                {
                    Id = x.Id,
                    MenuName = x.MenuName,
                    ParentId = x.ParentId,
                    Role = await GetRolesAsync(x.Id),
                    SubMenus = await GetSubMenusAsync(x.Id)
                }))).ToList();

            return new Response<List<MenuResponse>>(Constant.STATUS_SUCESS, null, dataResponse, dataResponse.Count());
        }

        /// <summary>
        /// kiểm tra menu có quyền k
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="identityRole"></param>
        /// <returns></returns>
        private bool IsMenuInRole(Menu menu, AppRole identityRole)
        {
            var menuRole = _newsContext.MenuRoles.Where(x => x.MenuId.Equals(menu.Id) && x.RoleId.Equals(identityRole.Id));
            if (menuRole != null && menuRole.Any())
                return true;

            return false;
        }
        
        /// <summary>
        /// lấy danh sách menu con của menu cha
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        private async Task<List<MenuResponse>> GetSubMenusAsync(Guid menuId)
        {
            var subMenusInfo = await _newsContext.Menus.Where(x => x.ParentId == menuId).ToListAsync();

            if (subMenusInfo.Count < 1 || subMenusInfo == null)
                return null;

            var subMenus = new List<MenuResponse>();

            foreach (var item in subMenusInfo)
            {
                subMenus.Add(new MenuResponse
                {
                    Id = item.Id,
                    MenuName = item.MenuName,
                    Role = await GetRolesAsync(item.Id),
                    ParentId = item.ParentId,
                    SubMenus = await GetSubMenusAsync(item.Id)
                });
            }

            return subMenus;
        }

        /// <summary>
        /// lấy danh sách quyền của menu
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        private async Task<List<string>> GetRolesAsync(Guid menuId)
        {
            var listRoleInfo = _newsContext.MenuRoles.Where(x => x.MenuId == menuId);

            if (listRoleInfo.Count() < 0 || listRoleInfo == null)
                return null;

            var roles = new List<string>();

            foreach (var roleId in listRoleInfo)
            {
                var role = await _roleManager.FindByIdAsync(roleId.RoleId.ToString());
                roles.Add(role.Name);
            }

            return roles;
        }
    }
}
