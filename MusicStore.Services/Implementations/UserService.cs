using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicStore.DataAccess;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Repositories.Interfaces;
using MusicStore.Services.Interfaces;
using MusicStore.Services.Utils;

namespace MusicStore.Services.Implementations;

public class UserService : IUserService
{
    private readonly UserManager<MusicStoreUserIdentity> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IOptions<AppSettings> _options;

    public UserService(UserManager<MusicStoreUserIdentity> userManager, RoleManager<IdentityRole> roleManager
        , ICustomerRepository customerRepository, ILogger<UserService> logger, IOptions<AppSettings> options)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _customerRepository = customerRepository;
        _logger = logger;
        _options = options;
    }

    public Task<LoginDtoResponse>? LoginAsync(LoginDtoRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<BaseResponseGeneric<string>> RegisterAsync(RegisterDtoRequest request)
    {
        var response = new BaseResponseGeneric<string>();

        try
        {
            var user = new MusicStoreUserIdentity
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Age = request.Age,
                DocumentNumber = request.DocumentNumber,
                DocumentType = (DocumentTypeEnum)request.DocumentType,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.ConfirmPassword);

            if (result.Succeeded)
            {
                var userIdentity = await _userManager.FindByEmailAsync(request.Email);

                if (userIdentity != null)
                {
                    var requestRole = request.Role ?? "Administrador";

                    if (!await _roleManager.RoleExistsAsync(requestRole))
                        await _roleManager.CreateAsync(new IdentityRole(requestRole));

                    await _userManager.AddToRoleAsync(userIdentity, requestRole);

                    var customer = new Customer
                    {
                        FullName = $"{request.FirstName} {request.LastName}",
                        Email = request.Email
                    };

                    await _customerRepository.AddAsync(customer);

                    response.Success = true;
                    response.Data = user.Id;
                }
            }
            else
            {
                response.Success = false;
                var sb = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    sb.AppendLine(error.Description);
                }

                response.ErrorMessage = sb.ToString();
                sb.Clear();

            }
        }
        catch (Exception ex)
        {
            response.ErrorMessage = _logger.LogMessage(ex, nameof(RegisterAsync));
        }

        return response;
    }

    public Task<BaseResponse> RequestTokenToResetPasswordAsync(DtoRequestPassword request)
    {
        throw new NotImplementedException();
    }

    public Task<BaseResponse> ResetPasswordAsync(DtoResetPassword request)
    {
        throw new NotImplementedException();
    }

    public Task<BaseResponse> ChangePasswordAsync(DtoChangePassword request)
    {
        throw new NotImplementedException();
    }
}