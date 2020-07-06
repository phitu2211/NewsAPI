using System;
using System.Collections.Generic;
using System.Text;

namespace NewsAPI.Contracts.V1.Model
{
    public class RoleModel
    {
        public string RoleName { get; set; }
        public string Description { get; set; }
    }

    public class RoleResponse : RoleModel
    {
        public Guid Id { get; set; }
    }

    public class RoleRequest : RoleModel
    {
    }

    public class RoleUpdateRequest : RoleModel
    {
        public List<string> UserId { get; set; }
        public List<Guid> MenuId { get; set; }
    }

    public class RoleQueryFilter : RoleModel
    {
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
        public RoleQueryFilter()
        {
            PageNumber = 1;
            PageSize = 10;
        }
        public RoleQueryFilter(int pageSize, int pageNumber)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class UpdateRoleAccountRequest
    {
        public List<Guid> AddAccountIds { get; set; }
        public List<Guid> RemoveAccountIds { get; set; }
    }

    public class UpdateRoleMenuRequest
    {
        public List<Guid> AddMenuIds { get; set; }
        public List<Guid> RemoveMenuIds { get; set; }
    }
}
