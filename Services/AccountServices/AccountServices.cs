using Caching;
using Core.Exceptions;
using Core.Helpers;
using Core.Interfaces;
using Data.Contexts;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models.DTOs.Account;
using Models.Enums;
using Models.Models;
using Models.ResponseModels;
using Models.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
namespace Services.AccountServices
{
    public class AccountServices: IAccountServices
    {

        private static readonly Random random = new Random();
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JWTSettings _jwtSettings;
        private readonly IConfiguration _conf;
        private readonly ICacheManager _cacheManager;
        private readonly IEmailCoreService _emailService;
        private readonly GoogleSetting _goolgeSetting;
        private readonly ApplicationDbContext _context;
        public AccountServices(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JWTSettings> jwtSettings,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration conf,
            ICacheManager cacheManager,
            IOptions<GoogleSetting> goolgeSetting,
            IEmailCoreService emailCoreService,
            ApplicationDbContext context
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
            _conf = conf;
            _cacheManager = cacheManager;
            _goolgeSetting = goolgeSetting.Value;
            _emailService = emailCoreService;
            _context = context;

        }

        public async Task<BaseResponse<InfoUser>> GetInfoUser(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                throw new ApiException($"Not found user id '{id}'.") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            var infoUser = new InfoUser
            {
                Id = user.Id,
                FullName = user.FullName,
                Address = user.Address,
                Email = user.Email,
                UserName = user.UserName,
                Phone =  user.PhoneNumber,
                DisplayName = user.DislayName,
                Province = user.Province,
                District = user.District,
                Ward = user.Ward

            };
            return new BaseResponse<InfoUser>(infoUser, "Info user");
        }

        public async Task<BaseResponse<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(request.UserNameOrEmail.Trim());
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(request.UserNameOrEmail);
                if(user == null)
                {
                    throw new ApiException($"You are not registered with '{request.UserNameOrEmail}'.") { StatusCode = (int)HttpStatusCode.BadRequest };

                }
            }
            

            SignInResult signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, lockoutOnFailure: false);
            if (!signInResult.Succeeded)
            {
                throw new ApiException($"Invalid Credentials for '{request.UserNameOrEmail}'.") { StatusCode = (int)HttpStatusCode.BadRequest };
            }

            string ipAddress = IpHelper.GetIpAddress();
            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user, ipAddress);
            AuthenticationResponse response = new AuthenticationResponse();
            response.Id = user.Id.ToString();
            response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.Email = user.Email;
            response.UserName = user.UserName;
            response.DisplayName = user.DislayName;
            IList<string> rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            response.Roles = rolesList.ToList();
            response.IsVerified = user.EmailConfirmed;
            response.RefreshToken = await GenerateRefreshToken(user);
            return new BaseResponse<AuthenticationResponse>(response, $"Authenticated {user.UserName}");
        }

        public async Task<BaseResponse<AuthenticationResponse>> LoginExternal(ExternalAuthDto request)
        {
            var payload = await VerifyGoogleToken(request);
            if (payload == null)
            {
                
                //return BadRequest("Invalid External Authentication.");
            }
            var info = new UserLoginInfo(request.Provider, payload.Subject, request.Provider);
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new ApplicationUser { Email = payload.Email, UserName = payload.Email };
                    await _userManager.CreateAsync(user);
                    await _userManager.AddLoginAsync(user, info);
                }
                else
                {
                    await _userManager.AddLoginAsync(user, info);
                }
            }
            string ipAddress = IpHelper.GetIpAddress();
            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user, ipAddress);
            AuthenticationResponse response = new AuthenticationResponse();
            response.Id = user.Id.ToString();
            response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.Email = user.Email;
            response.UserName = user.UserName;
            IList<string> rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            response.Roles = rolesList.ToList();
            response.IsVerified = user.EmailConfirmed;
            response.RefreshToken = await GenerateRefreshToken(user);
            return new BaseResponse<AuthenticationResponse>(response, $"Authenticated {user.UserName}");
        }
            private async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(ExternalAuthDto externalAuth)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _goolgeSetting.clientId }
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.IdToken, settings);
                return payload;
            }
            catch (Exception ex)
            {
                //log an exception
                return null;
            }
        }
        public async Task<BaseResponse<string>> ConfirmEmailAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return new BaseResponse<string>(user.Id.ToString(), message: $"Account Confirmed for {user.Email}. You can now use the /api/Account/authenticate endpoint.");
            }
            else
            {
                throw new ApiException($"An error occured while confirming {user.Email}.") { StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        }

        public async Task<BaseResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {

                    throw new ApiException($"Not found email {request.Email}.") { StatusCode = (int)HttpStatusCode.NotFound };
                }

                string password = ramdomPassword(10);
                var result = await _userManager.RemovePasswordAsync(user);
                if (!result.Succeeded)
                {
                    throw new ApiException($"Error in processing!") { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                result = await _userManager.AddPasswordAsync(user, password);
                if (!result.Succeeded)
                {
                    throw new ApiException($"Error in processing!") { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                await transaction.CommitAsync();
                string body = GetEmailBody(user.UserName , password);
                await _emailService.SendAsync(new Models.DTOs.Email.EmailRequest
                {
                    From = "nguyendinh.viet2002np@gmail.com",
                    To= request.Email,
                    Subject="Show tv reset passord", 
                    Body= body
                });
                return new BaseResponse<string>(request.Email, message: $"Password has been reset. Please check email");
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException($"An error {ex.Message}.") { StatusCode = (int)HttpStatusCode.InternalServerError };

            }


        }
        // ramdom password when forgot password 
        private string ramdomPassword(int length)
        {
            if (length < 4)
                throw new ArgumentException("Password length must be at least 4 to include all character types.");

            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string specialChars = "!@#$%^&*()_+-=[]{}|;:'\",.<>?/`~";
            const string digitChars = "0123456789";

            string allChars = lowerChars + upperChars + specialChars + digitChars;

            // ramdom phải có 1 ký tự thuộc 1 kiểu bất kỳ
            char[] password = new char[length];
            password[0] = lowerChars[random.Next(lowerChars.Length)];
            password[1] = upperChars[random.Next(upperChars.Length)];
            password[2] = specialChars[random.Next(specialChars.Length)];
            password[3] = digitChars[random.Next(digitChars.Length)];

            for (int i = 4; i < length; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }

            return new string(password.OrderBy(c => random.Next()).ToArray());
        }

        public Task<List<ApplicationUser>> GetUsers()
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponse<string>> LogoutAsync(string userEmail)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(userEmail);
            if (user != null)
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
            }
            await _signInManager.SignOutAsync();

            return new BaseResponse<string>(userEmail, message: $"Logout.");
        }

        public async Task<BaseResponse<AuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new ApiException($"You are not registered with '{request.Email}'.") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            //if (!user.EmailConfirmed)
            //{
            //    throw new ApiException($"Account Not Confirmed for '{request.Email}'.") { StatusCode = (int)HttpStatusCode.BadRequest };
            //}
            var tokenRefreshKey = $"TOKEN_REFRESH_KEY:{user.Id}";
            var getRefreshToken = await _cacheManager.GetAsync(tokenRefreshKey);
          
            if (string.IsNullOrWhiteSpace(getRefreshToken))
            {

                throw new ApiException($"Your token expired.") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            string tokenAffterTrim = getRefreshToken.Trim('"');
            bool isValid = await _userManager.VerifyUserTokenAsync(user, "MyApp", "RefreshToken", request.Token);
            if(tokenAffterTrim != request.Token || !isValid)
            {
                throw new ApiException($"Your token is not valid.") { StatusCode = (int)HttpStatusCode.BadRequest };

            }    
                string ipAddress = IpHelper.GetIpAddress();
            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user, ipAddress);
            AuthenticationResponse response = new AuthenticationResponse();
            response.Id = user.Id.ToString();
            response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.Email = user.Email;
            response.UserName = user.DislayName;
            IList<string> rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            response.Roles = rolesList.ToList();
            response.IsVerified = user.EmailConfirmed;
            response.RefreshToken = await GenerateRefreshToken(user);


            await _signInManager.SignInAsync(user, false);
            return new BaseResponse<AuthenticationResponse>(response, $"Authenticated {user.UserName}");
        }

        public async Task<BaseResponse<string>> RegisterAsync(RegisterRequest request, string uri)
        {
            ApplicationUser findUser = await _userManager.FindByNameAsync(request.UserName);
            if (findUser != null)
            {
                throw new ApiException($"Username '{request.UserName}' is already taken.") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            findUser = await _userManager.FindByEmailAsync(request.Email);
            if (findUser != null)
            {
                throw new ApiException($"Email {request.Email} is already registered.") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            ApplicationUser newUser = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.UserName,
                DislayName = request.Email
                
            };
            var result = await _userManager.CreateAsync(newUser, request.Password); 
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, RoleEnums.User.ToString());
                //var verificationUri = await SendVerificationEmail(newUser, uri);

                return new BaseResponse<string>(newUser.Id.ToString(), message: $"User Registered. Please confirm your account by visiting this URL");
            }
            else
            {
                throw new ApiException($"{result.Errors}") { StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        }

        public async Task<BaseResponse<string>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null) throw new ApiException($"You are not registered with '{request.Email}'.");
            // check password
            var passwordSuccess = await _userManager.CheckPasswordAsync(user,request.CurrentPassword);
            if (!passwordSuccess) throw new ApiException($"Current password not correct '{request.Email}'.");
            var result = await _userManager.RemovePasswordAsync(user);
            if (!result.Succeeded)
            {
                throw new ApiException($"Error in processing!") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            result = await _userManager.AddPasswordAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new ApiException($"Error in processing!") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            return new BaseResponse<string>("reset password is success");
        }

        //public async Task<List<ApplicationUser>> GetUsers()
        //{
        //    //return await _userManager.Users.ToListAsync();

        //    return await _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ToListAsync(); //lazzyloading
        //}

        private async Task<JwtSecurityToken> GenerateJWToken(ApplicationUser user, string ipAddress)
        {
            var claims = await GetValidClaims(user);
            claims.Add(new Claim("ip", ipAddress));
            _conf.GetSection(nameof(JWTSettings)).Bind(_jwtSettings);
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),// 130 phuts
                //expires: DateTime.UtcNow.AddSeconds(5),// 130 phuts

                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

        private async Task<string> GenerateRefreshToken(ApplicationUser user)
        {
            try
            {  //await _userManager.RemoveAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
                //IdentityResult result = await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", newRefreshToken);


                // set token in cache
                var tokenRefreshKey = $"TOKEN_REFRESH_KEY:{user.Id}";
                //var newRefreshToken = RandomTokenString();
                var tokenRefresh = await _userManager.GenerateUserTokenAsync(user, "MyApp", "RefreshToken");
                var getRefreshToken = await _cacheManager.GetAsync(tokenRefreshKey);
                await _cacheManager.SetAsync(tokenRefreshKey, tokenRefresh, 4320);
                return tokenRefresh;

            }
            catch(Exception ex)
            {
                throw new ApiException($"An error occured while set refreshtoken.") { StatusCode = (int)HttpStatusCode.InternalServerError };
            }
          
        }
        private string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }
        private async Task<List<Claim>> GetValidClaims(ApplicationUser user)
        {
            IdentityOptions _options = new IdentityOptions();
            var claims = new List<Claim>
            {  new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id.ToString()),
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            claims.AddRange(userClaims);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (Claim roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }
            return claims;
        }

        public async Task<BaseResponse<string>> UpdateInfoAsync(UpdateInfoRequest payload)
        {
            ApplicationUser findUser = await _userManager.FindByIdAsync(payload.Id);
            
            if (findUser != null)
            {
                if(payload.UserName != findUser.UserName)
                {
                    ApplicationUser checkUserName = await _userManager.FindByNameAsync(payload.UserName);
                    if (checkUserName != null)
                    {
                        throw new ApiException($"Username '{payload.UserName}' is already taken.") { StatusCode = (int)HttpStatusCode.BadRequest };
                    }
                }
            }
            findUser.UserName = payload.UserName;
            findUser.Address = payload.Address;
            findUser.FullName = payload.FullName;
            findUser.PhoneNumber = payload.Phone;
            findUser.Province = payload.Province;
            findUser.Ward = payload.Ward;
            findUser.District = payload.District;
            var result = await _userManager.UpdateAsync(findUser);
            if (result.Succeeded)
            {
                return new BaseResponse<string>(findUser.Id.ToString(), message: $"User Updated successfully");
            }
            else
            {
                throw new ApiException($"{result.Errors}") { StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        }
        private string GetEmailBody(string username, string newPassword)
        {
            return $@"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Password Reset</title>
        </head>
        <body>
            <h1>Reset Your Password</h1>
            <p>Dear {username},</p>
            <p>You requested to reset your password. Your new password is:</p>
            <p><strong>{newPassword}</strong></p>
            <p>If you did not request this, please contact our support team immediately.</p>
            <p>Thanks, <br /> The Team</p>
        </body>
        </html>";
        }
    }
}
