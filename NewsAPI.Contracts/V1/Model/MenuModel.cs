using System;
using System.Collections.Generic;

namespace NewsAPI.Contracts.V1.Model
{
    public class MenuModel
    {
        public string MenuName { get; set; }
    }

    public class GetMenusByRoleNameRequest
    {
        public List<string> RoleNames { get; set; }
    }

    public class MenuQueryFilter : MenuModel
    {
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
        public MenuQueryFilter()
        {
            PageNumber = 1;
            PageSize = 10;
        }
        public MenuQueryFilter(int pageSize, int pageNumber)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class MenuRequest : MenuModel
    {
        public Guid? ParentId { get; set; }
        public List<string> Role { get; set; }
    }

    public class MenuUpdateRequest : MenuModel
    {
        public Guid? ParentId { get; set; }
        public List<string> AddRoles { get; set; }
        public List<string> RemoveRoles { get; set; }
    }

    public class MenuResponse : MenuModel
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public List<string> Role { get; set; }
        public List<MenuResponse> SubMenus { get; set; }
    }

    public class GetMenuByRoleIdResponse
    {
        public List<MenuResponse> Role { get; set; }
        public List<MenuResponse> NoRole { get; set; }
    }
}