using Application.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Supplier;
using Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplier;
        public SupplierController(ISupplierService supplier)
        {
            _supplier = supplier;
        }
        [HttpPost("list")]
        public async Task<IActionResult> GetSuppliers()
        {
            var result = await _supplier.GetSuppliers();
            if (result.Errors == null || !result.Errors.Any())
            {
            }
            return Ok(result);

        }
        [HttpPost("{id}")]
        public async Task<IActionResult> GetCatogories(Guid id)
        {
            var result = await _supplier.GetSupplier(id);
            if (result.Errors == null || !result.Errors.Any())
            {
            }
            return Ok(result);

        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateSupplier(SupplierRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _supplier.InsertSupplier(request);
            if (result.Errors == null || !result.Errors.Any())
            {
            }
            return Ok(result);
        }
        [HttpPut("update/{id}")]
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
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(Guid id)
        {
            if (id == null)
            {
                return BadRequest(new { message = "Id not null or empty" });
            }
            var result = await _supplier.DeleteSupplier(id);
            return Ok(result);
        }
    }
}
