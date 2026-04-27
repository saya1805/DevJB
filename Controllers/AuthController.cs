using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevJBackend.Data;
using DevJBackend.Entity;
using DevJBackend.Interface;
using DevJBackend.Model;
using DevJBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;    
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace DevJBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        //private static User? user = new();
        //private IConfiguration _config;
        //private DevDbContext context;
        //public AuthController(IConfiguration configuration,DevDbContext context)
        //{
        //    _config = configuration;
        //}

       
        private readonly IAuth _Service;
        private readonly DevDbContext _dbcontext;
        private readonly MailService _mailservice;
        private readonly IMemoryCache _memorycache;
        private readonly IWebHostEnvironment _env;

        public AuthController(IAuth authservice, DevDbContext devDbContext, MailService mailservice, IMemoryCache memoryCache, IWebHostEnvironment env)
        {
            _Service = authservice;
            _mailservice = mailservice;
            _dbcontext = devDbContext;
            _memorycache = memoryCache;
            _env = env;
        }




        //[HttpPost("signin")]
        //public ActionResult<User?> Signin(UserDeto request)
        //{
        //    user.fullname = request.fullname;
        //    user.username = request.username;
        //    user.mailid = request.mailid;
        //    user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.Password);
        //    return Ok(user);
        //}

        //[HttpPost("loginuser")]
        //public ActionResult<User?> Login(UserDeto request)
        //{
        //    if (user.username != request.username)
        //    {
        //        return BadRequest("User Not Found");
        //    }

        //    if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) ==
        //        PasswordVerificationResult.Failed)
        //    {
        //        return BadRequest("Wrong Password");
        //    }
        //    return Ok(user);
        //}


        [HttpPost("signin")]
        public async Task<ActionResult<User?>> Signin(UserDeto request)
        {
               var user = await _Service.Signinasync(request);
            return Ok(user);
        }


        //[HttpPost("loginuser")]
        //public ActionResult<string> Login(UserDeto request)
        //{
        //    if (user.username != request.username)
        //    {
        //        return BadRequest("User Not Found");
        //    }

        //    if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) ==
        //        PasswordVerificationResult.Failed)
        //    {
        //        return BadRequest("Wrong Password");
        //    }
        //    string token = CreateToken(user);
        //    return Ok(token);
        //}

        [HttpPost("loginuser")]
        public async Task<ActionResult<TokenResponseDeto?>> Login(UserDeto request)
        {
            var token = await _Service.Loginasync(request);
            if (token is null)
            {
                return BadRequest("Username & pasword is incorrect");
            }
            return token;
        }

        [HttpGet("get-users")]
        public async Task<ActionResult<List<UserDeto>>> Getallusers()
        {
            var users = await _dbcontext.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDeto?>> Login(RefreshTokenResponseDeto request)
        {
            var token = await _Service.RefreshTokenAsync(request);
            if (token is null)
            {
                return BadRequest("Token Expired");
            }
            return token;
        }

        //private string CreateToken(User user)
        //{
        //    var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name,user.username)
        //    };
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("AppSettings:Token")!));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
        //    var tokenstructure = new JwtSecurityToken(
        //        issuer: _config.GetValue<string>("AppSettings:Issuer"),
        //        audience: _config.GetValue<string>("AppSettings:Audience"),
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddMinutes(30),
        //        signingCredentials: creds
        //        );

        //    return new JwtSecurityTokenHandler().WriteToken(tokenstructure);
        //}

        [HttpGet("AuthEnd")]
        [Authorize]
        public ActionResult AuthCheck()
        {
            return Ok();
        }

        [HttpGet("AdminEnd")]
        [Authorize(Roles ="Admin")]
        public ActionResult AdminCheck()
        {
            return Ok();
        }

        //otp send api
        [HttpPost("SendOtp")]
        public IActionResult Sendotp(string email)
        {

            var UserExists = _dbcontext.Users.Any(x => x.mailid == email);

            if (UserExists)
            {
                return BadRequest(new { Message = "This email is already registered." });
            }

            Random rndm = new Random();

            var otp = rndm.Next(100000, 999999).ToString();
            var cacheoptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            _memorycache.Set(email, otp, cacheoptions);
            try
            {
                _mailservice.sendOtpmail(email, otp);
                return Ok(new { message = "Otp has been sent on your Registered Mail id" });
            }catch(Exception ex)
            {
                return Ok(new { message = "Could not send OTP. Please check your network and try again." + ex.Message});
            }
        }

        [HttpPost("VerifyOtp")]
        public IActionResult Verifyotp(string email, string Otp)
        {
            if(_memorycache.TryGetValue(email, out string storedOtp))
            {
                if(storedOtp == Otp)
                {
                    _memorycache.Remove(email);
                    return Ok(new { message = "Otp Verified Sucessfully" });
                }
                return BadRequest(new { message = "Invalid Otp!" });
            }
            return BadRequest(new { message = "Otp Expired or Not Found!" });
        }


        [HttpPost("UpdateprofilePicture")]
        public async Task<IActionResult> updateprofilepicture(IFormFile file)
        {

            if (file == null || file.Length == 0) return BadRequest("फाइल नाहीये!");

            // १. फाइल सेव्ह करायचा पाथ ठरवा
            //var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            //if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            // २. फाइल सर्व्हरवर सेव्ह करा
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // ३. डेटाबेसमध्ये फक्त 'fileName' सेव्ह करा (उदा. "xyz.jpg")
            // _userService.UpdateProfilePhoto(userId, fileName);

            return Ok(new { path = "uploads/" + fileName });


        }

    }
}
