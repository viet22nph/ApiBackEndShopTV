﻿using Microsoft.AspNetCore.Identity.Data;
using Models.DTOs.Account;
using Models.Models;
using Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AccountServices
{
    public interface IAccountServices
    {
        Task<BaseResponse<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request);
        Task<BaseResponse<string>> RegisterAsync(Models.DTOs.Account.RegisterRequest request, string uri);
        Task<BaseResponse<string>> ConfirmEmailAsync(string userId, string code);
        Task<BaseResponse<string>> ForgotPasswordAsync(Models.DTOs.Account.ForgotPasswordRequest request);
        Task<BaseResponse<string>> ResetPasswordAsync(Models.DTOs.Account.ResetPasswordRequest request);
        Task<BaseResponse<AuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequest request);
        Task<BaseResponse<string>> LogoutAsync(string userEmail);
        Task<List<ApplicationUser>> GetUsers();

        Task<BaseResponse<AuthenticationResponse>> LoginExternal(ExternalAuthDto request);
        Task<BaseResponse<string>> UpdateInfoAsync(UpdateInfoRequest payload);
        Task<BaseResponse<InfoUser>> GetInfoUser(string id);
    }
}
