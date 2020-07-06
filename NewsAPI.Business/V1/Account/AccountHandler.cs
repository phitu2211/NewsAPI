using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NewsAPI.Contracts.V1;
using NewsAPI.Contracts.V1.Helper;
using NewsAPI.Contracts.V1.Model;
using NewsAPI.Data.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace NewsAPI.Business.V1
{
    public class AccountHandler : IAccountHandler
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        private readonly IPasswordValidator<AppUser> _passwordValidator;
        private readonly IUserValidator<AppUser> _userValidator;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ILogger<AccountHandler> _logger;

        public AccountHandler(UserManager<AppUser> userManager, IPasswordHasher<AppUser> passwordHasher,
            IPasswordValidator<AppUser> passwordValidator, IUserValidator<AppUser> userValidator,
            RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager, ILogger<AccountHandler> logger)
        {
            _logger = logger;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _passwordHasher = passwordHasher;
            _passwordValidator = passwordValidator;
            _userValidator = userValidator;
            _userManager = userManager;
        }

        #region CRUD
        /// <summary>
        /// Đăng ký, tạo mới tài khoản
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Thông tin tài khoản</returns>
        public async Task<Response<AccountResponse>> RegisterAsync(AccountRegistrationRequest request)
        {
            if (request.Role.Count == 0)
                request.Role.Add(Constant.ROLE_USER);

            if (String.IsNullOrEmpty(request.Password))
            {
                _logger.LogError(Constant.PASS_NULL);
                return new Response<AccountResponse>(Constant.STATUS_ERROR, new List<string> { Constant.PASS_NULL });
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
            {
                _logger.LogError(Constant.USER_EXIST);
                return new Response<AccountResponse>
                {
                    Data = null,
                    Message = new List<string> { Constant.USER_EXIST },
                    Status = Constant.STATUS_ERROR,
                    TotalData = 0
                };
            }

            var newUser = new AppUser
            {
                Email = request.Email,
                UserName = request.FirstName + request.LastName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Age = request.Age,
                Address = request.Address,
                Id = Guid.NewGuid()
            };

            var createdUser = await _userManager.CreateAsync(newUser, request.Password);

            if (!createdUser.Succeeded)
            {
                _logger.LogError("Create User Failed");
                return new Response<AccountResponse>(Constant.STATUS_ERROR, createdUser.Errors.Select(x => x.Description));
            }
            await _userManager.AddToRolesAsync(newUser, request.Role);

            var dataResponse = new AccountResponse
            {
                Id = newUser.Id,
                Address = newUser.Address,
                Email = newUser.Email,
                Role = request.Role,
                Age = newUser.Age,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName
            };

            _logger.LogInformation("Create account success");
            return new Response<AccountResponse>(Constant.STATUS_SUCESS, null, dataResponse, 1);
        }

        /// <summary>
        /// Sửa thông tin tài khoản, thêm xóa quyền
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns>Thông tin tài khoản</returns>
        public async Task<Response<AccountResponse>> UpdateAccountAsync(Guid userId, UpdateAccountRequest request)
        {
            var appUser = await _userManager.FindByIdAsync(userId.ToString());
            if (appUser == null)
            {
                _logger.LogError(Constant.USER_NOT_EXIST);
                return new Response<AccountResponse>(Constant.STATUS_ERROR, new List<string> { Constant.USER_NOT_EXIST });
            }
            var validUser = await _userValidator.ValidateAsync(_userManager, appUser);

            if (!validUser.Succeeded || validUser == null)
            {
                _logger.LogError("User is not validate");
                return new Response<AccountResponse>(Constant.STATUS_ERROR, validUser.Errors.Select(x => x.Description));
            }
            if (request.Age > 0)
                appUser.Age = request.Age;

            if (!string.IsNullOrEmpty(request.Address))
                appUser.Address = request.Address;

            if (!string.IsNullOrEmpty(request.LastName))
                appUser.LastName = request.LastName;

            if (!string.IsNullOrEmpty(request.FirstName))
                appUser.FirstName = request.FirstName;

            appUser.UserName = appUser.FirstName + appUser.LastName;

            if (!string.IsNullOrEmpty(request.Email))
                appUser.Email = request.Email;

            if (!string.IsNullOrEmpty(request.Password))
            {
                var validPass = await _passwordValidator.ValidateAsync(_userManager, appUser, request.Password);
                if (validPass.Succeeded)
                    appUser.PasswordHash = _passwordHasher.HashPassword(appUser, request.Password);
                else
                {
                    _logger.LogError("Password is not validate");
                    return new Response<AccountResponse>(Constant.STATUS_ERROR, validPass.Errors.Select(x => x.Description));
                }
            }

            var result = new IdentityResult();

            //Add account to role
            foreach (var item in request.AddRoles ?? new List<string> { })
            {
                var role = await _roleManager.FindByNameAsync(item);
                if (role != null)
                {
                    result = await _userManager.AddToRoleAsync(appUser, role.Name);
                    if (!result.Succeeded)
                    {
                        _logger.LogError("Add user to role failed");
                        return new Response<AccountResponse>(Constant.STATUS_ERROR, result.Errors.Select(x => x.Description));
                    }
                }
            }

            // Xóa tài khoản khỏi quyền
            foreach (var item in request.RemoveRoles ?? new List<string> { })
            {
                var role = await _roleManager.FindByNameAsync(item);
                if (role != null)
                {
                    result = await _userManager.RemoveFromRoleAsync(appUser, role.Name);
                    if (!result.Succeeded)
                    {
                        _logger.LogError("Remove user from role failed");
                        return new Response<AccountResponse>(Constant.STATUS_ERROR, result.Errors.Select(x => x.Description));
                    }
                }
            }

            result = await _userManager.UpdateAsync(appUser);

            var dataResponse = new AccountResponse
            {
                Id = appUser.Id,
                Address = appUser.Address,
                Age = appUser.Age,
                Email = appUser.Email,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                Role = (await _userManager.GetRolesAsync(appUser)).ToList()
            };

            if (result.Succeeded)
            {
                _logger.LogInformation("Update account success");
                return new Response<AccountResponse>(Constant.STATUS_SUCESS, null, dataResponse, 1);
            }

            _logger.LogError("Update account error");
            return new Response<AccountResponse>(Constant.STATUS_ERROR, result.Errors.Select(x => x.Description));
        }

        /// <summary>
        /// Xóa tài khoản
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Sucess với data = null</returns>
        public async Task<Response<AccountResponse>> DeleteAccountAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                _logger.LogError(Constant.USER_NOT_EXIST);
                return new Response<AccountResponse>(Constant.STATUS_ERROR, new List<string> { Constant.USER_NOT_EXIST });
            }
            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogError("Delete account error");
                return new Response<AccountResponse>(Constant.STATUS_ERROR, result.Errors.Select(x => x.Description));
            }

            _logger.LogInformation("Delete account success");
            return new Response<AccountResponse>(Constant.STATUS_SUCESS);
        }

        /// <summary>
        /// Xem chi tiết tài khoản theo id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Thông tin tài khoản</returns>
        public async Task<Response<AccountResponse>> GetAccountByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                _logger.LogError(Constant.USER_NOT_EXIST);
                return new Response<AccountResponse>(Constant.STATUS_ERROR, new List<string> { Constant.USER_NOT_EXIST });
            }
            var dataResponse = new AccountResponse
            {
                Id = user.Id,
                Address = user.Address,
                Age = user.Age,
                Email = user.Email,
                LastName = user.LastName,
                FirstName = user.FirstName,
                Role = (await _userManager.GetRolesAsync(user)).ToList()
            };

            return new Response<AccountResponse>(Constant.STATUS_SUCESS, null, dataResponse, 1);
        }

        /// <summary>
        /// Lấy danh sách tài khoản theo filter: tên, địa chỉ, tuổi,..
        /// Phân trang tài khoản
        /// Lấy tất cả gửi pageSize = null, pageNumber = null
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Danh sách tài khoản theo filter</returns>
        public async Task<PaginatedList<AccountResponse>> GetAccountByFilterAsync(AccountQueryFilter filter)
        {
            _logger.LogInformation("Test log");

            var dataResponse = new List<AccountResponse>();

            foreach (var user in _userManager.Users)
            {
                dataResponse.Add(new AccountResponse
                {
                    Id = user.Id,
                    Address = user.Address,
                    Age = user.Age,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = (await _userManager.GetRolesAsync(user)).ToList()
                });
            }

            if (!string.IsNullOrEmpty(filter.Address))
                dataResponse = dataResponse.Where(x => x.Address.Contains(filter.Address)).ToList();

            if (!string.IsNullOrEmpty(filter.Email))
                dataResponse = dataResponse.Where(x => x.Email.Equals(filter.Email)).ToList();

            if (!string.IsNullOrEmpty(filter.FirstName))
                dataResponse = dataResponse.Where(x => x.FirstName.Contains(filter.FirstName)).ToList();

            if (!string.IsNullOrEmpty(filter.LastName))
                dataResponse = dataResponse.Where(x => x.LastName.Contains(filter.LastName)).ToList();

            if (filter.Age > 0)
                dataResponse = dataResponse.Where(x => x.Age.Equals(filter.Age)).ToList();

            if (!filter.PageSize.HasValue && !filter.PageNumber.HasValue)
                return await PaginatedList<AccountResponse>.CreateAsync(dataResponse);

            return await PaginatedList<AccountResponse>.CreateAsync(dataResponse, filter.PageNumber.Value, filter.PageSize.Value);
        }
        #endregion

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Thông tin tài khoản đăng nhập</returns>
        public async Task<Response<AccountResponse>> LoginAsync(AccountLoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                _logger.LogError(Constant.USER_NOT_EXIST);
                return new Response<AccountResponse>(Constant.STATUS_ERROR, new List<string> { Constant.USER_NOT_EXIST });
            }

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);

            if (!result.Succeeded)
            {
                _logger.LogError("Login Failed");
                return new Response<AccountResponse>(Constant.STATUS_ERROR, new List<string> { "Login Failed" });
            }
            var dataResponse = new AccountResponse
            {
                Id = user.Id,
                Address = user.Address,
                Age = user.Age,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = (await _userManager.GetRolesAsync(user)).ToList()
            };

            return new Response<AccountResponse>(Constant.STATUS_SUCESS, null, dataResponse, 1);
        }

        /// <summary>
        /// Lấy danh sách tài khoản theo role id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>Danh sách thông tin tài khoản theo role id</returns>
        public async Task<Response<GetAccountsByRoleIdResponse>> GetAccountsByRoleId(Guid roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                _logger.LogError("No find role with id");
                return new Response<GetAccountsByRoleIdResponse>(Constant.STATUS_ERROR, new List<string> { "No find role with id" });
            }

            var dataResponse = new GetAccountsByRoleIdResponse();
            //Ds user có role
            var members = new List<AccountResponse>();
            //Ds user không có role
            var nonMembers = new List<AccountResponse>();

            //Thêm user vào 2 ds
            foreach (var user in _userManager.Users)
            {
                var list = await _userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                list.Add(new AccountResponse
                {
                    Id = user.Id,
                    Address = user.Address,
                    Age = user.Age,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = (await _userManager.GetRolesAsync(user)).ToList()
                });
            }

            dataResponse.Members = members;
            dataResponse.NonMembers = nonMembers;

            return new Response<GetAccountsByRoleIdResponse>(Constant.STATUS_SUCESS, null, dataResponse, 1);
        }
    }
}
