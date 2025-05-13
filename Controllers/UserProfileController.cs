using CakeStore.Data;
using CakeStore.Models;
using CakeStore.Models.Dtos;

namespace CakeStore.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/user-profile")]
public class UserProfileController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserProfileController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null) return NotFound();

        return Ok(profile);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(UserProfileDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

        var existing = await _context.UserProfiles.AnyAsync(p => p.UserId == userId);
        if (existing) return BadRequest("Profile already exists");

        var profile = new UserProfile
        {
            UserId = userId,
            FullName = dto.FullName,
            Email = dto.Email
        };

        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();
        return Ok(profile);
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> Update(UserProfileDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null) return NotFound();

        profile.FullName = dto.FullName;
        profile.Email = dto.Email;

        await _context.SaveChangesAsync();
        return Ok(profile);
    }
}
