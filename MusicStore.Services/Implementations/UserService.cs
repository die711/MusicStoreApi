using Microsoft.AspNetCore.Identity;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Services.Interfaces;

namespace MusicStore.Services.Implementations;

public class UserService : IUserService
{
    public Task<LoginDtoResponse>? LoginAsync(LoginDtoRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<BaseResponseGeneric<string>> RegisterAsync(RegisterDtoRequest request)
    {
        throw new NotImplementedException();
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