using Core.Exceptions;
using Core.Helpers;
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
namespace Services.AccountServices
{
    public class AccountServices: IAccountServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JWTSettings _jwtSettings;
        private readonly IConfiguration _conf;
        public AccountServices(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JWTSettings> jwtSettings,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration conf
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
            _conf = conf;
        }
        public async Task<BaseResponse<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(request.Email.Trim());
            if (user == null)
            {
                throw new ApiException($"You are not registered with '{request.Email}'.") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            if (!user.EmailConfirmed)
            {
                throw new ApiException($"Account Not Confirmed for '{request.Email}'.") { StatusCode = (int)HttpStatusCode.BadRequest };
            }

            SignInResult signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, lockoutOnFailure: false);
            if (!signInResult.Succeeded)
            {
                throw new ApiException($"Invalid Credentials for '{request.Email}'.") { StatusCode = (int)HttpStatusCode.BadRequest };
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

        public Task ForgotPasswordAsync(ForgotPasswordRequest request, string uri)
        {
            throw new NotImplementedException();
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

            string refreshToken = await _userManager.GetAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
            bool isValid = await _userManager.VerifyUserTokenAsync(user, "MyApp", "RefreshToken", request.Token);
            if (!refreshToken.Equals(request.Token) || !isValid)
            {
                throw new ApiException($"Your token is not valid.") { StatusCode = (int)HttpStatusCode.BadRequest };
            }

            string ipAddress = IpHelper.GetIpAddress();
            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user, ipAddress);
            AuthenticationResponse response = new AuthenticationResponse();
            response.Id = user.Id.ToString();
            response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.Email = user.Email;
            response.UserName = user.UserName;
            //IList<string> rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            //response.Roles = rolesList.ToList();
            //response.IsVerified = user.EmailConfirmed;
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
                UserName = request.UserName
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

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
            if (result.Succeeded)
            {
                return new BaseResponse<string>(request.Email, message: $"Password Resetted.");
            }
            else
            {
                throw new ApiException($"Error occured while reseting the password. Please try again.");
            }
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
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

        private async Task<string> GenerateRefreshToken(ApplicationUser user)
        {
            await _userManager.RemoveAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
            var newRefreshToken = await _userManager.GenerateUserTokenAsync(user, "MyApp", "RefreshToken");
            IdentityResult result = await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", newRefreshToken);
            if (!result.Succeeded)
            {
                throw new ApiException($"An error occured while set refreshtoken.") { StatusCode = (int)HttpStatusCode.InternalServerError };
            }
            return newRefreshToken;
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
    }
}
