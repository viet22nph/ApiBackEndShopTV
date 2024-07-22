using Application.DAL.Models;
using Caching;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Supplier;
using Models.Settings;
using Services.Interfaces;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplier;
        private readonly ICacheManager _cacheManager;
        public SupplierController(ISupplierService supplier, ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            _supplier = supplier;
        }
        [HttpPost("list")]
        [Cache]
        public async Task<IActionResult> GetSuppliers()
        {
            var result = await _supplier.GetSuppliers();
          
            return Ok(result);

        }
        [HttpPost("{id}")]
        [Cache]
        public async Task<IActionResult> GetSupplier(Guid id)
        {
            var result = await _supplier.GetSupplier(id);
          
            return Ok(result);

        }
        [HttpPost("create")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Supplier.Create)]
        public async Task<IActionResult> CreateSupplier(SupplierRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _cacheManager.RemoveByPrefix("api/Supplier");
            var result = await _supplier.InsertSupplier(request);
            
            return Ok(result);
        }
        [HttpPut("update/{id}")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Supplier.Update)]
        public async Task<IActionResult> UpdateSupplier(Guid id,[FromBody] SupplierRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(id == null)
            {
                return BadRequest(new { message = "Id not null or empty" });
            }
            var result = await _supplier.UpdateSupplier(id,request);
            _cacheManager.RemoveByPrefix("api/Supplier");
            return Ok(result);
        }
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = Permissions.Supplier.Delete)]
        public async Task<IActionResult> DeleteSupplier(Guid id)
        {
            if (id == null)
            {
                return BadRequest(new { message = "Id not null or empty" });
            }
            var result = await _supplier.DeleteSupplier(id);
            _cacheManager.RemoveByPrefix("api/Supplier");
            return Ok(result);
        }
    }
}
