using Data.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.UserManagement;
using Models.Models;
using Models.Settings;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleManagerController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public RoleManagerController(UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager,
           ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("get-roles")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.RoleManager.Read)]
        public async Task<IActionResult> GetRoles()
        {
            var roles =  _roleManager.Roles.Select(r=> new {id = r.Id, name = r.Name}).OrderBy(c=>c.name).ToList();
            if(!roles.Any() )
            {
                  return BadRequest(new { message = $"No roles" });
            }

            return Ok(new {message="Roles",data = roles});
        }
        [HttpPost("get-role-by-id/{id}")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.RoleManager.Read)]
        public async Task<IActionResult> GetRoleById(string id)
        {
            
            var role =await  _roleManager.FindByIdAsync(id);
            if(role == null)
            {
                return NotFound(new { message = $"No role by id {id}" });
            }
            var claims = await _roleManager.GetClaimsAsync(role);

            return Ok(new
            {
                message = "Roles",
                data = new
                {
                    id = role.Id,
                    name = role.Name,
                    claims = claims.ToList()
                }
            });
        }
        [HttpPost("create-role")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.RoleManager.Create)]
        public async Task<IActionResult> CreateRole([FromBody] RoleName roleName)
        {

            if (string.IsNullOrWhiteSpace(roleName.Name))
            {
                return BadRequest(new { message = $"Role name is null or empty" });
            }
            try
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName.Name);
                if (roleExist)
                {
                    return BadRequest(new { message = $"Internal server error: The {roleName.Name} role already exist" });

                }
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName.Name));
                if (!roleResult.Succeeded)
                {
                    return BadRequest(new { message = $"Internal server error: Create role {roleName.Name} is failed" });
                }


                return Ok(new { message = $"Create role {roleName.Name} is successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Internal server error: " + ex.Message });
            }

        }
        [HttpPatch("update-role-name/{id}")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.RoleManager.Update)]
        public async Task<IActionResult> UpdateRoleName(string id, [FromBody] RoleName payload)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return BadRequest(new { message = $"Role not found" });
            }
            if(string.IsNullOrWhiteSpace(payload.Name))
            {
                return BadRequest(new { message = $"Role name is null or white space" });
            }
            if(await _roleManager.RoleExistsAsync(payload.Name))
            {

                return BadRequest(new { message = $"Role {payload.Name} already exist" });
            }    
           
            role.Name = payload.Name;
            await _roleManager.UpdateAsync(role);

            return Ok(new {message=$"Update role {payload.Name} successfully"});
        }
        [HttpDelete("removeRole/{roleName}")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.RoleManager.Delete)]
        public async Task<IActionResult> RemoveRoles(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return BadRequest(new { message = $"Role not found" });
            }

            var result = await _roleManager.DeleteAsync(role);
            if(!result.Succeeded)
            {
                return BadRequest(new { message = $"Remove role {roleName } is failed" });
            }

            return Ok(new { message = $"Remove role {roleName} is successfully" });
        }

        [HttpPost("add-role-claims")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.RoleManager.Create)]
        public async Task<IActionResult> AddRoleClaims([FromBody] AddRoleClaimsRequest request)
        {
            await _context.Database.BeginTransactionAsync();
            try
            {
                // Check if the role exists
                if (!await _roleManager.RoleExistsAsync(request.RoleName))
                {
                    return BadRequest(new { message = $"Role not found" });
                }
                var role = await _roleManager.FindByNameAsync(request.RoleName);
                var existingClaim = await _roleManager.GetClaimsAsync(role);
                foreach (var roleClaim in request.RoleClaims)
                {
                    if (existingClaim.Any(c => c.Type == roleClaim.ClaimType && c.Value == roleClaim.ClaimValue))
                    {
                        await _context.Database.RollbackTransactionAsync();
                        return BadRequest(new { message = $"Role claim {roleClaim.ClaimType}-{roleClaim.ClaimValue} eists" });
                    }
                    var claim = new Claim(roleClaim.ClaimType, roleClaim.ClaimValue);
                    await _roleManager.AddClaimAsync(role, claim);
                }
                await _context.Database.CommitTransactionAsync();
                return Ok(new { message = "Add role claims successfully" });
            }
            catch (Exception ex)
            {
                await _context.Database.RollbackTransactionAsync();
                return BadRequest(new { message = "Internal server error: " + ex.Message });

            }
        }
        [HttpDelete("remove-role-claim/{roleName}")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.RoleManager.Delete)]
        public async Task<IActionResult> RemoveRoleClaim(string roleName, string claimType, string claimValue)
        {
            try
            {
                // Find the role by name
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    return NotFound(new { message = $"Role with name '{roleName}' not found" });
                }

                var claims = await _roleManager.GetClaimsAsync(role);
                var claimToRemove = claims.FirstOrDefault(c => c.Type == claimType && c.Value == claimValue);

                if (claimToRemove == null)
                {
                    return NotFound(new { message = $"Claim with type '{claimType}' and value '{claimValue}' not found in role '{roleName}'" });
                }

                var result = await _roleManager.RemoveClaimAsync(role, claimToRemove);
                if (!result.Succeeded)
                {
                    return BadRequest(new { message = $"Claim with type '{claimType}' and value '{claimValue}' in role '{roleName}' remove failed" });
                }
                return Ok(new { message = $"Claim with type '{claimType}' and value '{claimValue}' removed from role '{roleName}'" });
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = "Internal server error: " + ex.Message });
            }
        }
        [HttpPost("get-role-claim-by-role/{id}")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.RoleManager.Read)]
        public async Task<IActionResult> GetRoleClaimsByRole(string id)
        {


            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return BadRequest(new { message = $"Role not found" });
            }

            var roleClaims = await _roleManager.GetClaimsAsync(role);
            var res = roleClaims.Select(cl=> new {type = cl.Type, value = cl.Value});
            return Ok(new {message ="Role Claims", data = res});
        }
    }
    public class RoleName
    {
        public string Name { get; set; }
    }
}
