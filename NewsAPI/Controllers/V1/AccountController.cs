﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAPI.Business.V1;
using NewsAPI.Contracts.V1.Helper;
using NewsAPI.Contracts.V1.Model;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace NewsAPI.Controllers.V1
{
    [Produces("application/json")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountHandler _accountService;

        public AccountController(IAccountHandler accountService)
        {
            _accountService = accountService;
        }

        [HttpPost(Constant.ApiRoutes.Account.Register)]
        public async Task<Response<AuthenticationResult>> Register([FromBody] AccountRegistrationRequest request)
        {
            return await _accountService.RegisterAsync(request);
        }

        [HttpPost(Constant.ApiRoutes.Account.Login)]
        public async Task<Response<AuthenticationResult>> Login([FromBody] AccountLoginRequest request)
        {
            return await _accountService.LoginAsync(request);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut(Constant.ApiRoutes.Account.UpdateAccount)]
        public async Task<Response<AccountResponse>> UpdateAccount([FromRoute] Guid userId, [FromBody] UpdateAccountRequest request)
        {
            return await _accountService.UpdateAccountAsync(userId, request);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet(Constant.ApiRoutes.Account.GetAccountByFilter)]
        public async Task<PaginatedList<AccountResponse>> GetAccountByFilter(string filter = "{}")
        {
            var filterConvert = JsonConvert.DeserializeObject<AccountQueryFilter>(filter);

            return await _accountService.GetAccountByFilterAsync(filterConvert);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet(Constant.ApiRoutes.Account.GetAccountById)]
        public async Task<Response<AccountResponse>> GetAccountById([FromRoute] Guid userId)
        {
            return await _accountService.GetAccountByIdAsync(userId);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete(Constant.ApiRoutes.Account.DeleteAccount)]
        public async Task<Response<AccountResponse>> DeleteAccount([FromRoute] Guid userId)
        {
            return await _accountService.DeleteAccountAsync(userId);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet(Constant.ApiRoutes.Account.GetAccountsByRoleId)]
        public async Task<Response<GetAccountsByRoleIdResponse>> GetAccountsByRoleId([FromRoute] Guid roleId)
        {
            return await _accountService.GetAccountsByRoleId(roleId);
        }
    }
}