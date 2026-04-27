using DevJBackend.Entity;
using DevJBackend.Model;
using Microsoft.AspNetCore.Mvc;

namespace DevJBackend.Interface
{
    public interface IAuth
    {
        Task<User?> Signinasync(UserDeto request);
        Task<TokenResponseDeto?> Loginasync(UserDeto request);
        Task<ActionResult<TokenResponseDeto?>> RefreshTokenAsync(RefreshTokenResponseDeto request);
    }
}
