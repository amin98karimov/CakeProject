using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CakeStore.Data;
using CakeStore.Models;
using CakeStore.Models.Auth;
using CakeStore.Models.Dtos;

[ApiController]
[Route("cake-store")]
public class CakeStoreController : ControllerBase
{
    private readonly AppDbContext _context;

    public CakeStoreController(AppDbContext context)
    {
        _context = context;
    }

    // GET /cake-store/list
    [HttpGet("list")]
    public async Task<IActionResult> ListCakes()
    {
        var cakes = await _context.Cakes
            .Include(c => c.Properties)
            .ToListAsync();

        return Ok(cakes);
    }

    // POST /cake-store/buy
    [Authorize(Roles = "Customer")]
    [HttpPost("buy")]
    public async Task<IActionResult> BuyCake([FromBody] BuyCakeDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

        if (!int.TryParse(userIdStr, out int userId))
            return BadRequest("Invalid user ID");

        var cake = await _context.Cakes.FindAsync(dto.CakeId);
        if (cake == null) return NotFound("Cake not found");

        var userCake = new UserCake
        {
            UserId = userId,
            CakeId = cake.Id,
            BoughtAt = DateTime.UtcNow
        };

        _context.UserCakes.Add(userCake);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Cake bought successfully!" });
    }
    
    [Authorize(Roles = "Baker")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateCake([FromBody] CreateCakeDto dto)
    {
        var cake = new Cake
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Properties = dto.Properties.Select(p => new CakeProperty
            {
                Name = p.Name,
                Value = p.Value
            }).ToList()
        };

        _context.Cakes.Add(cake);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Cake created", cake.Id });
    }
    
    [Authorize(Roles = "Customer")]
    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized();

        var orders = await _context.UserCakes
            .Where(uc => uc.UserId == userId)
            .Include(uc => uc.Cake)
            .ThenInclude(c => c.Properties)
            .OrderByDescending(uc => uc.BoughtAt)
            .ToListAsync();

        var result = orders.Select(uc => new
        {
            CakeName = uc.Cake.Name,
            uc.Cake.Description,
            uc.Cake.Price,
            Properties = uc.Cake.Properties.Select(p => new { p.Name, p.Value }),
            BoughtAt = uc.BoughtAt
        });

        return Ok(result);
    }
    
    [Authorize(Roles = "Customer")]
    [HttpPost("review")]
    public async Task<IActionResult> LeaveReview([FromBody] ReviewDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var cakeExists = await _context.Cakes.AnyAsync(c => c.Id == dto.CakeId);
        if (!cakeExists) return NotFound("Cake not found");

        if (dto.Rating < 1 || dto.Rating > 5)
            return BadRequest("Rating must be between 1 and 5");

        var review = new CakeReview
        {
            UserId = userId,
            CakeId = dto.CakeId,
            Rating = dto.Rating,
            Comment = dto.Comment
        };

        _context.CakeReviews.Add(review);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Review submitted" });
    }

    [HttpGet("{cakeId}/reviews")]
    public async Task<IActionResult> GetReviews(int cakeId)
    {
        var reviews = await _context.CakeReviews
            .Where(r => r.CakeId == cakeId)
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        var result = reviews.Select(r => new
        {
            r.Comment,
            r.Rating,
            r.CreatedAt,
            Reviewer = r.User.Username
        });

        return Ok(result);
    }



}