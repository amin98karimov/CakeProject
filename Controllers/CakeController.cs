using CakeStore.Data;
using CakeStore.Models;
using CakeStore.Models.Dtos;

namespace CakeStore.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CakeController : ControllerBase
{
    private readonly AppDbContext _context;

    public CakeController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("list")]
    public async Task<IActionResult> ListCakes()
    {
        var cakes = await _context.Cakes
            .Include(c => c.Properties)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Description,
                c.Price,
                Properties = c.Properties.Select(p => new { p.Name, p.Value }),
                AverageRating = _context.CakeReviews
                    .Where(r => r.CakeId == c.Id)
                    .Average(r => (double?)r.Rating) ?? 0.0
            })
            .ToListAsync();

        return Ok(cakes);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetCakeById(int id)
    {
        var cake = await _context.Cakes
            .Where(c => c.Id == id)
            .Include(c => c.Properties)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Description,
                c.Price,
                Properties = c.Properties.Select(p => new { p.Name, p.Value }),
                AverageRating = _context.CakeReviews
                    .Where(r => r.CakeId == c.Id)
                    .Average(r => (double?)r.Rating) ?? 0.0
            })
            .FirstOrDefaultAsync();

        if (cake == null) return NotFound();
        return Ok(cake);
    }


    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, CakeDto dto)
    {
        var cake = await _context.Cakes.Include(c => c.Properties).FirstOrDefaultAsync(c => c.Id == id);
        if (cake == null) return NotFound();

        cake.Name = dto.Name;
        cake.Description = dto.Description;
        cake.Price = dto.Price;

        // Clear and replace properties
        _context.CakeProperties.RemoveRange(cake.Properties);
        cake.Properties = dto.Properties.Select(p => new CakeProperty
        {
            Name = p.Name,
            Value = p.Value
        }).ToList();

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var cake = await _context.Cakes.Include(c => c.Properties).FirstOrDefaultAsync(c => c.Id == id);
        if (cake == null) return NotFound();

        _context.CakeProperties.RemoveRange(cake.Properties);
        _context.Cakes.Remove(cake);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
