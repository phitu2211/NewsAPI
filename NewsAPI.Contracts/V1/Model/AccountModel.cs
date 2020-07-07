using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewsAPI.Contracts.V1.Model
{
    public class AccountModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }
    }

    public class GetAccountsByRoleIdResponse
    {
        public List<AccountResponse> Members { get; set; }
        public List<AccountResponse> NonMembers { get; set; }
    }

    public class AccountResponse : AccountModel
    {
        public Guid Id { get; set; }
        public List<string> Role { get; set; }
    }

    public class UpdateAccountRequest : AccountModel
    {
        public string Password { get; set; }
        public List<string> AddRoles { get; set; }
        public List<string> RemoveRoles { get; set; }
    }

    public class AccountLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AccountQueryFilter : AccountModel
    {
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
        public AccountQueryFilter()
        {
            PageNumber = 1;
            PageSize = 10;
        }
        public AccountQueryFilter(int pageSize, int pageNumber)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class AccountRegistrationRequest : AccountModel
    {
        public string Password { get; set; }
        public List<string> Role { get; set; } = new List<string>();
    }

    public class AuthenticationResult
    {
        public string Token { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public string RefreshToken { get; set; }
    }
}
