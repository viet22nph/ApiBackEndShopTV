
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Account;
using Models.Settings;
using Services.AccountServices;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountServices _accountService;
        public AccountController(IAccountServices accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync(AuthenticationRequest request)
        {
            //auth
            var result = await _accountService.AuthenticateAsync(request);
            
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            var uri = $"{Request.Scheme}://{Request.Host.Value}";
            return Ok(await _accountService.RegisterAsync(request, uri));
        }
        //doi lam gui email roi lam
        //[HttpGet("confirm-email")]
        //public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code)
        //{
        //    return Ok(await _accountService.ConfirmEmailAsync(userId, code));
        //}

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var uri = $"{Request.Scheme}://{Request.Host.Value}";
            await _accountService.ForgotPasswordAsync(request);
            return Ok();
        }

        [HttpPost("reset-password")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            return Ok(await _accountService.ResetPasswordAsync(request));
        }

        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenRequest request)
        {
            return Ok(await _accountService.RefreshTokenAsync(request));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("logout")]
        public async Task<IActionResult> LogoutAsync(string userEmail)
        {
            return Ok(await _accountService.LogoutAsync(userEmail));
        }
        [HttpPost("ExternalLogin")]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalAuthDto externalAuth)
        {
            var result = await _accountService.LoginExternal(externalAuth);

            return Ok(result);
        }

    }
}
