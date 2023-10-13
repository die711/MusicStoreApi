using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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

    public async Task<LoginDtoResponse> LoginAsync(LoginDtoRequest request)
    {
        var response = new LoginDtoResponse();

        try
        {
            var identity =await _userManager.FindByEmailAsync(request.UserName);

            if (identity == null)
                throw new SecurityException("Usuario no existe");

            if (await _userManager.IsLockedOutAsync(identity))
                throw new SecurityException(
                    $"Demasiados intentos fallidos para el usuario {identity.UserName}, reintente mas tarde");

            var result = await _userManager.CheckPasswordAsync(identity, request.Password);
            if (!result)
            {
                response.Success = false;
                response.ErrorMessage = "Clave incorrecta";
                _logger.LogWarning("Error de autenticacion para el usuario {Username}", request.UserName);

                return response;
            }

            var roles = await _userManager.GetRolesAsync(identity);
            var expiredDate = DateTime.Now.AddDays(1);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, request.UserName),
                new Claim(ClaimTypes.Email, request.UserName),
                new Claim(ClaimTypes.Expiration, expiredDate.ToString("yyyy-MM-dd HH:mm:ss")),
            };
            
            claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)));

            response.Roles = new List<string>();
            response.Roles.AddRange(roles);
            
            //Creacion de JWT
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.Jwt.SecretKey));

            var credentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            var header = new JwtHeader(credentials);

            var payload = new JwtPayload(_options.Value.Jwt.Issuer, _options.Value.Jwt.Audience, claims
                , DateTime.Now,
                expiredDate);

            var token = new JwtSecurityToken(header, payload);
            response.Token = new JwtSecurityTokenHandler().WriteToken(token);
            response.FullName = $"{identity.FirstName} {identity.LastName}";
            response.Success = true;
        }
        catch (SecurityException ex)
        {
            response.ErrorMessage = _logger.LogMessage(ex, "Error de autenticacion", false);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = _logger.LogMessage(ex, nameof(LoginAsync));
        }

        return response;
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