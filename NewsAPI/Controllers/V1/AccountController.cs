using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewsAPI.Contracts.V1.Helper;
using NewsAPI.Contracts.V1.Model;
using Newtonsoft.Json;
using NewsAPI.Business.V1;
using Microsoft.Extensions.Logging;

namespace NewsAPI.Controllers.V1
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountHandler _accountService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(IAccountHandler accountService, ILogger<AccountController> logger)
        {
            _logger = logger;
            _accountService = accountService;
        }

        [HttpPost(Constant.ApiRoutes.Account.Register)]
        public async Task<Response<AccountResponse>> Register([FromBody] AccountRegistrationRequest request)
        {
            return await _accountService.RegisterAsync(request);
        }

        [HttpPost(Constant.ApiRoutes.Account.Login)]
        public async Task<Response<AccountResponse>> Login([FromBody] AccountLoginRequest request)
        {
            return await _accountService.LoginAsync(request);
        }

        [HttpPut(Constant.ApiRoutes.Account.UpdateAccount)]
        public async Task<Response<AccountResponse>> UpdateAccount([FromRoute] Guid userId, [FromBody] UpdateAccountRequest request)
        {
            return await _accountService.UpdateAccountAsync(userId, request);
        }

        [HttpGet(Constant.ApiRoutes.Account.GetAccountByFilter)]
        public async Task<PaginatedList<AccountResponse>> GetAccountByFilter(string filter = "{}")
        {
            _logger.LogInformation("Test log");
            var filterConvert = JsonConvert.DeserializeObject<AccountQueryFilter>(filter);

            return await _accountService.GetAccountByFilterAsync(filterConvert);
        }

        [HttpGet(Constant.ApiRoutes.Account.GetAccountById)]
        public async Task<Response<AccountResponse>> GetAccountById([FromRoute] Guid userId)
        {
            return await _accountService.GetAccountByIdAsync(userId);
        }

        [HttpDelete(Constant.ApiRoutes.Account.DeleteAccount)]
        public async Task<Response<AccountResponse>> DeleteAccount([FromRoute] Guid userId)
        {
            return await _accountService.DeleteAccountAsync(userId);
        }

        [HttpGet(Constant.ApiRoutes.Account.GetAccountsByRoleId)]
        public async Task<Response<GetAccountsByRoleIdResponse>> GetAccountsByRoleId([FromRoute] Guid roleId)
        {
            return await _accountService.GetAccountsByRoleId(roleId);
        }
    }
}