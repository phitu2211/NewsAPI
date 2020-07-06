using NewsAPI.Contracts.V1.Model;
using NewsAPI.Contracts.V1;
using NewsAPI.Contracts.V1.Helper;
using System.Threading.Tasks;
using System;

namespace NewsAPI.Business.V1
{
    public interface IAccountHandler
    {
        Task<Response<AccountResponse>> RegisterAsync(AccountRegistrationRequest request);
        Task<Response<AccountResponse>> LoginAsync(AccountLoginRequest request);
        Task<PaginatedList<AccountResponse>> GetAccountByFilterAsync(AccountQueryFilter filter);
        Task<Response<AccountResponse>> GetAccountByIdAsync(Guid userId);
        Task<Response<AccountResponse>> UpdateAccountAsync(Guid userId, UpdateAccountRequest request);
        Task<Response<AccountResponse>> DeleteAccountAsync(Guid userId);
        Task<Response<GetAccountsByRoleIdResponse>> GetAccountsByRoleId(Guid roleId);
    }
}
