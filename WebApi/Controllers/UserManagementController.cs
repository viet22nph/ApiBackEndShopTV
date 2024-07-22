using Caching;
using CloudinaryDotNet.Actions;
using Core.Exceptions;
using Data.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.User;
using Models.DTOs.UserManagement;
using Models.Models;
using Models.Settings;
using Org.BouncyCastle.Utilities;
using Services.Interfaces;
using System.Net;
using System.Security.Claims;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly ICacheManager _cacheManager;
        public UserManagementController(UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager,
           ApplicationDbContext context,
           IUserService userService,
           ICacheManager cacheManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _cacheManager = cacheManager;
            _userService = userService;
        }

        [HttpPost("add-role-to-user")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.UserManager.Create)]
        public async Task<IActionResult> AddRoleToUser([FromBody] AddRoleToUserRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    return NotFound(new { message = $"User with Id = {request.UserId} cannot be found" });
                }

                if (!await _roleManager.RoleExistsAsync(request.RoleName))
                {
                    return BadRequest(new { message = $"Role {request.RoleName} does not exist" });
                }
                if(await _userManager.IsInRoleAsync(user, request.RoleName))
                {
                    return BadRequest(new { message = $"User already exists in this role" });
                }

                var result = await _userManager.AddToRoleAsync(user, request.RoleName);

                if (!result.Succeeded)
                {
                    return BadRequest(new { message = $"Internal server error: Add {request.RoleName} To User is failed" });
                }
                _cacheManager.RemoveByPrefix("api/UserManagement");
                return Ok(new { message = $"User {user.UserName} has been added to role {request.RoleName}" });
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = "Internal server error: " + ex.Message });
            }
          
        }
        [HttpPost("get-user-roles")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.UserManager.Read)]
        public async Task<IActionResult> GetRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return BadRequest(new {message=$"Not found user with '{userId}'"});
            }
            var roles =  await _userManager.GetRolesAsync(user);
            return Ok(new {mesage="Roles",
                data=roles
            });
        }
        [HttpPost("get-user")]
        [Cache(300)]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.UserManager.Read)]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
           
            if (user == null)
            {
                return BadRequest(new { message = $"Not found user with '{userId}'" });
            }
            var roles = await _userManager.GetRolesAsync(user);
            var res = new UserDetailDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Address = user.Address,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Roles = roles
            };
            return Ok(res);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.UserManager.Read)]
        [HttpPost("get-users")]
        [Cache]
        public async Task<IActionResult> GetUsers(int pageNumber =1, int pageSize=10)
        {
            var result = await _userService.GetUsers(pageNumber, pageSize);
            return Ok(result);
        }
        [HttpDelete("user-remove-role")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.UserManager.Delete)]
        public async Task<IActionResult> UserRemoveRole([FromBody] UserRemoveRoleRequest request)
        {
            try
            {
                await _context.Database.BeginTransactionAsync();
                var user = await _userManager.FindByIdAsync(request.userId);
                if (user == null)
                {
                    return BadRequest(new
                    {
                        message = $"Not found user by id {request.userId}"
                    });
                }
                if (!request.RoleNames.Any())
                {
                    return BadRequest(new
                    {
                        message = $"Please provide roles"
                    });
                }
                foreach (var roleName in request.RoleNames)
                {
                    if (!await _userManager.IsInRoleAsync(user, roleName))
                    {
                        await _context.Database.RollbackTransactionAsync();
                        return BadRequest($"User is not in role '{roleName}'.");

                    }
                    var result = await _userManager.RemoveFromRoleAsync(user, roleName);
                    if (!result.Succeeded)
                    {
                        await _context.Database.RollbackTransactionAsync();
                        return BadRequest($"Failed to remove user from role {roleName}.");
                    }
                }

                var roles = await _userManager.GetRolesAsync(user);
                return Ok(new
                {
                    message = "Remove role success",
                    data = roles
                });
            }catch(Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}")
                { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            
        }


        public class UserRemoveRoleRequest
        {
            public string userId { get; set; }
            public ICollection<String> RoleNames { get; set; }
        }
    }
}
