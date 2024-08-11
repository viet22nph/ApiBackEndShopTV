using Application.DAL.Models;
using Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.DTOs.Product;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        public ColorController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetAllColor()
        {
            var color = await _context.Set<Color>().ToListAsync();
            return Ok(new
            {
                message = "Color",
                data = color.Select(color => new
                {
                    Id = color.Id,
                    ColorName = color.ColorName,
                    ColorCode = color.ColorCode,
                }).ToList()
            });
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create(ColorRequestDto payload)
        {
            var color =  await _context.Set<Color>()
                .Where(x => x.ColorCode == payload.ColorCode || x.ColorName == payload.ColorName)
                .FirstOrDefaultAsync();
            if (color != null)
            {
                return BadRequest(new
                {
                    message = $"Exists color name = {payload.ColorName} or color code = {payload.ColorCode}"
                }) ;
            }
            var colorData = new Color
            {
                ColorName = payload.ColorName,
                ColorCode = payload.ColorCode,
                DateCreate = DateTime.UtcNow
            };
            _context.Set<Color>().Add(colorData);
            _context.SaveChanges();
            return Created();

        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(ColorDto payload)
        {
            var color = await _context.Set<Color>()
                .FirstOrDefaultAsync(x=> x.Id == payload.Id);
            if (color == null)
            {
                return NotFound(new
                {
                    message = $"Not found"
                });
            }
            color.ColorName = payload.ColorName;
            color.ColorCode = payload.ColorCode;
            color.DateUpdate = DateTime.UtcNow;
           
            _context.Set<Color>().Update(color);
            _context.SaveChanges();
            return Created();

        }



    }

    public class ColorRequestDto {
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
    }
}
