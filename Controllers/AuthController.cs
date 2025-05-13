using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using CakeStore.Data;
using CakeStore.Models;
using CakeStore.Models.Auth;
using CakeStore.Services;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public AuthController(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            return BadRequest("Username already exists");

        using var sha = SHA256.Create();
        var passwordBytes = Encoding.UTF8.GetBytes(dto.Password);
        var passwordHash = Convert.ToBase64String(sha.ComputeHash(passwordBytes));

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = passwordHash,
            UserType = dto.UserType
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User registered successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null) return Unauthorized("Invalid credentials");

        using var sha = SHA256.Create();
        var passwordBytes = Encoding.UTF8.GetBytes(dto.Password);
        var passwordHash = Convert.ToBase64String(sha.ComputeHash(passwordBytes));

        if (user.PasswordHash != passwordHash)
            return Unauthorized("Invalid credentials");

        var token = _tokenService.CreateToken(user);
        return Ok(new { token });
    }


    }