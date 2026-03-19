using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;
using System.Text;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MusicStore.DataAccess;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Services.Interfaces;
using MusicStore.Services.Utils;

namespace MusicStore.Services.Implementations;

public class UserService : IUserService
{
    private readonly UserManager<MusicStoreUserIdentity> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly MusicStoreDbContext _context;
    private readonly ILogger<UserService> _logger;
    private readonly IOptions<AppSettings> _options;
    private readonly IEmailService _emailService;

    public UserService(UserManager<MusicStoreUserIdentity> userManager, RoleManager<IdentityRole> roleManager
        , MusicStoreDbContext context, ILogger<UserService> logger, IOptions<AppSettings> options, IEmailService emailService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _logger = logger;
        _options = options;
        _emailService = emailService;
    }

    public async Task<LoginDtoResponse> LoginAsync(LoginDtoRequest request)
    {
        var response = new LoginDtoResponse();

        var identity = await _userManager.FindByEmailAsync(request.UserName);

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

        return response;
    }

    public async Task<BaseResponseGeneric<string>> RegisterAsync(RegisterDtoRequest request)
    {
        var response = new BaseResponseGeneric<string>();

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

                await _context.Set<Customer>().AddAsync(customer);
                await _context.SaveChangesAsync();


                await _emailService.SendEmailAsync(request.Email, $"Creacion de cuenta para {customer.FullName}",
                    $@"
                        <p>
                            Se ha creado su cuenta <strong>{user.UserName}</strong> exitosamente en nuestro sistema, bienvenido
                        </p>
                    ");


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

        return response;
    }

    public async Task<BaseResponse> RequestTokenToResetPasswordAsync(DtoRequestPassword request)
    {
        var response = new BaseResponse();

        var userIdentity = await _userManager.FindByEmailAsync(request.Email);

        if (userIdentity is null)
            throw new ApplicationException("Usuario no existe");

        var token = await _userManager.GeneratePasswordResetTokenAsync(userIdentity);

        await _emailService.SendEmailAsync("di_564@hotmail.com", "Reestablecer contraseña", $@"
            <h3> {userIdentity.FirstName}, se ha solicitado el reseteo de tu contraseña </h3>
            <p> para reestablecer tu password copia y pega este token y no lo compartes con nadie. <strong> {token} </strong> </p>
        ");

        _logger.LogInformation("Se envio correo con solicitud para reseteo de la cuenta {Email}", request.Email);
        _logger.LogInformation("tu reset token es {Token}", token);
        response.Success = true;

        return response;
    }

    public async Task<BaseResponse> ResetPasswordAsync(DtoResetPassword request)
    {
        var response = new BaseResponse();
        var userIdentity = await _userManager.FindByEmailAsync(request.Email);

        if (userIdentity is null)
            throw new ApplicationException("Usuario no existe");

        var result = await _userManager.ResetPasswordAsync(userIdentity, request.Token, request.NewPassword);

        if (!result.Succeeded)
        {
            var sb = new StringBuilder();
            foreach (var error in result.Errors)
            {
                sb.AppendLine(error.Description);
            }

            response.ErrorMessage = sb.ToString();
            sb.Clear();
        }
        else
        {

        }

        return response;
    }

    public async Task<BaseResponse> ChangePasswordAsync(DtoChangePassword request)
    {
        var response = new BaseResponse();

        var userIdentity = await _userManager.FindByEmailAsync(request.Email);
        if (userIdentity == null)
            throw new ApplicationException("Usuario no existe");

        var result = await _userManager.ChangePasswordAsync(userIdentity, request.OldPassword, request.NewPassword);
        response.Success = result.Succeeded;

        if (!result.Succeeded)
        {
            var sb = new StringBuilder();
            foreach (var error in result.Errors)
            {
                sb.AppendLine(error.Description);
            }

            response.ErrorMessage = sb.ToString();
            sb.Clear();
        }

        return response;
    }
}