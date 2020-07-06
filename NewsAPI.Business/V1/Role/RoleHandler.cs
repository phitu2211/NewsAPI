using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NewsAPI.Contracts.V1.Model;
using NewsAPI.Contracts.V1;
using NewsAPI.Contracts.V1.Helper;
using NewsAPI.Data;
using NewsAPI.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NewsAPI.Business.V1
{
    public class RoleHandler : IRoleHandler
    {
        private readonly NewsContext _newsContext;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<RoleHandler> _logger;

        public RoleHandler(NewsContext newsContext, RoleManager<AppRole> roleManager, UserManager<AppUser> userManager, ILogger<RoleHandler> logger)
        {
            _logger = logger;
            _userManager = userManager;
            _newsContext = newsContext;
            _roleManager = roleManager;
        }

        #region CRUD
        /// <summary>
        /// Tạo mới quyền
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Response<RoleResponse>> CreateRoleAsync(RoleRequest request)
        {
            if (string.IsNullOrEmpty(request.RoleName))
            {
                _logger.LogError("Name role is not null");
                return new Response<RoleResponse>(Constant.STATUS_ERROR, new List<string> { "Name role is not null" });
            }

            if (string.IsNullOrEmpty(request.Description))
            {
                _logger.LogError("Description is not null");
                return new Response<RoleResponse>(Constant.STATUS_ERROR, new List<string> { "Description is not null" });
            }

            var identityRole = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = request.RoleName,
                Description = request.Description
            };

            var result = await _roleManager.CreateAsync(identityRole);

            var dataResponse = new RoleResponse
            {
                Id = identityRole.Id,
                RoleName = identityRole.Name,
                Description = identityRole.Description
            };

            if (result.Succeeded)
            {
                _logger.LogInformation("Create role sucess");
                return new Response<RoleResponse>(Constant.STATUS_SUCESS, null, dataResponse, 1);
            }

            _logger.LogInformation("Create role error");
            return new Response<RoleResponse>(Constant.STATUS_ERROR, result.Errors.Select(x => x.Description));
        }
        
        /// <summary>
        /// Xóa quyền theo id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<Response<RoleResponse>> DeleteRoleAsync(Guid roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());

            if (role == null)
            {
                _logger.LogError("Not find role with id");
                return new Response<RoleResponse>(Constant.STATUS_ERROR, new List<string> { "Not find role with id" });
            }

            var listUser = await _userManager.Users.ToListAsync();

            foreach (var user in listUser)
            {
                if(await _userManager.IsInRoleAsync(user, role.Name))
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
            }

            var menuRoles = _newsContext.MenuRoles.Where(x => x.RoleId.Equals(roleId));

            if (menuRoles != null || menuRoles.Count() > 0)
                _newsContext.MenuRoles.RemoveRange(menuRoles);

            IdentityResult result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                _logger.LogInformation("Delete role sucess");
                return new Response<RoleResponse>(Constant.STATUS_SUCESS);
            }

            _logger.LogError("Delete role error");
            return new Response<RoleResponse>(Constant.STATUS_ERROR, result.Errors.Select(x => x.Description));
        }

        /// <summary>
        /// Xem chi tiết quyền theo id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<Response<RoleResponse>> GetRoleByIdAsync(Guid roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());

            if (role == null)
            {
                _logger.LogError("Not find role with id");
                return new Response<RoleResponse>(Constant.STATUS_ERROR, new List<string> { "Not find role with id" });
            }

            var dataResponse = new RoleResponse
            {
                Id = role.Id,
                RoleName = role.Name,
                Description = role.Description
            };
            return new Response<RoleResponse>(Constant.STATUS_SUCESS, null, dataResponse, 1);
        }

        /// <summary>
        /// Xem danh sách quyền theo filter:
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<PaginatedList<RoleResponse>> GetRoleByFilterAsync(RoleQueryFilter filter)
        {
            var dataResponse = new List<RoleResponse>();

            dataResponse.AddRange(_roleManager.Roles.Select(x => new RoleResponse 
            {
                RoleName = x.Name, 
                Id = x.Id,
                Description = x.Description
            }).ToList());

            if (!string.IsNullOrEmpty(filter.RoleName))
                dataResponse = dataResponse.Where(x => x.RoleName == filter.RoleName).ToList();

            if (!string.IsNullOrEmpty(filter.Description))
                dataResponse = dataResponse.Where(x => x.Description.Contains(filter.Description)).ToList();

            if (!filter.PageSize.HasValue && !filter.PageNumber.HasValue)
                return await PaginatedList<RoleResponse>.CreateAsync(dataResponse);

            return await PaginatedList<RoleResponse>.CreateAsync(dataResponse, filter.PageNumber.Value, filter.PageSize.Value);
        }

        /// <summary>
        /// Cập nhật quyền
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Response<RoleResponse>> UpdateRoleAsync(Guid roleId, RoleRequest request)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());

            if (role == null)
            {
                _logger.LogError("Not find role with id");
                return new Response<RoleResponse>(Constant.STATUS_ERROR, new List<string> { "Not find role with id" });
            }

            if (!string.IsNullOrEmpty(request.RoleName))
                role.Name = request.RoleName;

            var result = await _roleManager.UpdateAsync(role);

            var dataResponse = new RoleResponse
            {
                Id = role.Id,
                RoleName = role.Name,
                Description = role.Description
            };

            if (result.Succeeded)
            {
                _logger.LogInformation("Update role sucess");
                return new Response<RoleResponse>(Constant.STATUS_SUCESS, null, dataResponse, 1);
            }

            _logger.LogError("Update role error");
            return new Response<RoleResponse>(Constant.STATUS_ERROR, result.Errors.Select(x => x.Description));
        }
        
        #endregion

        /// <summary>
        /// Cập nhật quyền menu: thêm xóa menu khỏi quyền
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Response<RoleResponse>> UpdateRoleMenuAsync(Guid roleId, UpdateRoleMenuRequest request)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());

            if (role == null)
            {
                _logger.LogError("Not find role with id");
                return new Response<RoleResponse>(Constant.STATUS_ERROR, new List<string> { "Not find role with id" });
            }

            //thêm menu vào quyền
            foreach (var item in request.AddMenuIds ?? new List<Guid> { })
            {
                var menu = await _newsContext.Menus.FindAsync(item);
                if(menu != null)
                {
                    var menuRole = _newsContext.MenuRoles.Where(x => x.RoleId.Equals(role.Id) && x.MenuId.Equals(menu.Id));
                    if (menuRole != null && menuRole.Any())
                    {
                        _logger.LogError($"Menu already in role '{role.Name}'");
                        return new Response<RoleResponse>(Constant.STATUS_ERROR, new List<string> { $"Menu already in role '{role.Name}'" });
                    }
                    await _newsContext.MenuRoles.AddAsync(new MenuRole
                    {
                        Id = Guid.NewGuid(),
                        MenuId = menu.Id,
                        RoleId = role.Id
                    });
                }
            }

            //xóa menu khỏi quyền
            foreach (var item in request.RemoveMenuIds ?? new List<Guid> { })
            {
                var menu = await _newsContext.Menus.FindAsync(item);
                if (menu != null)
                {
                    var menuRole = _newsContext.MenuRoles.Where(x => x.RoleId == role.Id && x.MenuId == menu.Id);
                    _newsContext.MenuRoles.RemoveRange(menuRole);
                }
            }

            var updated = await _newsContext.SaveChangesAsync();

            if (updated > 0)
            {
                _logger.LogInformation("Update role menu sucess");
                return new Response<RoleResponse>(Constant.STATUS_SUCESS);
            }

            _logger.LogError("Update role menu failed");
            return new Response<RoleResponse>(Constant.STATUS_ERROR, new List<string> { "Error when save" });
        }

        /// <summary>
        /// Cập nhật quyền tài khoản: thêm xóa tài khoản vào quyền
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Response<RoleResponse>> UpdateRoleAccountAsync(Guid roleId, UpdateRoleAccountRequest request)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());

            if (role == null)
            {
                _logger.LogError("Not find role with id");
                return new Response<RoleResponse>(Constant.STATUS_ERROR, new List<string> { "Not find role with id" });
            }

            //Thêm tài khoản vào quyền
            foreach (var item in request.AddAccountIds ?? new List<Guid> { })
            {
                var account = await _userManager.FindByIdAsync(item.ToString());
                if (account != null)
                {
                    var result = await _userManager.AddToRoleAsync(account, role.Name);
                    if (!result.Succeeded)
                    {
                        _logger.LogError("Add user to role failed");
                        return new Response<RoleResponse>(Constant.STATUS_ERROR, result.Errors.Select(x => x.Description));
                    }
                }
            }

            //Xóa tài khoản khỏi quyền
            foreach (var item in request.RemoveAccountIds ?? new List<Guid> { })
            {
                var account = await _userManager.FindByIdAsync(item.ToString());
                if (account != null)
                {
                    var result = await _userManager.RemoveFromRoleAsync(account, role.Name);
                    if (!result.Succeeded)
                    {
                        _logger.LogError("Remove user from role failed");
                        return new Response<RoleResponse>(Constant.STATUS_ERROR, result.Errors.Select(x => x.Description));
                    }
                }
            }

            _logger.LogInformation("Update role menu sucess");
            return new Response<RoleResponse>(Constant.STATUS_SUCESS);
        }
    }
}
