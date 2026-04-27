using DevJBackend.Data;
using DevJBackend.Entity;
using DevJBackend.Interface;
using DevJBackend.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DevJBackend.Services
{
    public class DevService : IAuth
    {
        //[Route("api/[controller]")]
        //[ApiController]

        //public class AuthService : ControllerBase
        //{
        private static User? user = new();
        private IConfiguration _config;
        private DevDbContext _context;
        public DevService(IConfiguration configuration, DevDbContext context)
        {
            _config = configuration;
            this._context = context;
        }

        //[HttpPost("signin")]
        public async Task<User?> Signinasync(UserDeto request)
        {
            var userexists = await _context.Users.AnyAsync(u => u.username == request.username);
            //if (await _context.Users.AnyAsync(u => u.username == request.username))
                if (userexists)
                {
                    return null;
                }
            var user = new User();
            user.fullname = request.fullname;
            user.username = request.username;
            user.mailid = request.mailid;
            user.Roles = "Admin";
            user.ProfilePictureUrl = request.ProfilePictureUrl;
            user.RefreshToken = "";
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.Password);
            //await _context.Users.AddAsync(user);
            //await _context.SaveChangesAsync();
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // इथे ब्रेकपॉईंट लावा आणि 'ex.InnerException.Message' वाचा
                var errorMessage = ex.Message;
                throw;
            }
            return user;
        }

        //[HttpPost("loginuser")]


        //[HttpPost("loginuser")]
        //public async Task<string> Loginasync(UserDeto request)
        //{
        //    User? user = await _context.Users.FirstOrDefaultAsync(u => u.username == request.username);
        //        if (user is null)
        //    {
        //        return null;
        //    }

        //    if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) ==
        //        PasswordVerificationResult.Failed)
        //    {
        //        return null;
        //    }
        //    string token = CreateToken(user);
        //    return token;
        //}

        public async Task<TokenResponseDeto> Loginasync(UserDeto request)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.username == request.username);
            if (user is null)
            {
                return null;
            }
            var result = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return new TokenResponseDeto
            {
                Roles = user.Roles,
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateRefreshToken(user)
            };
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.username),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Role,user.Roles),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("AppSettings:Token")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var tokenstructure = new JwtSecurityToken(
                issuer: _config.GetValue<string>("AppSettings:Issuer"),
                audience: _config.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenstructure);
            //}

        }

        private async Task<string> GenerateRefreshToken(User user)
        {
            var randomNumber = new byte[32];
            using var reng = RandomNumberGenerator.Create();
            reng.GetBytes(randomNumber);
            var refreshToken = Convert.ToBase64String(randomNumber);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
            await _context.SaveChangesAsync();
            return refreshToken;
        }


        public async Task<ActionResult<TokenResponseDeto?>> RefreshTokenAsync(RefreshTokenResponseDeto request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                return null;
            }
            var token = new TokenResponseDeto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateRefreshToken(user),
            };
            return new OkObjectResult(token);

        }

    }

}
