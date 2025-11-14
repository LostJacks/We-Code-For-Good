using System.Security.Claims;
using System.Security.Cryptography;
using LabelingTool.BLL.DTOs;
using LabelingTool.BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration.UserSecrets;
using Model;

namespace Smart_Server.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ITokenService tokenService,
        AppDbContext dbContext,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _dbContext = dbContext;
        _configuration = configuration;
    }
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequestDTO request)
    {
        var user = new AppUser
        {
            UserName = request.UserName,
            Email = request.Email


        };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        return await GenerateAuthResponse(user);
    }
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequestDTO request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            return Unauthorized("Invalid username or password.");
        }
        return await GenerateAuthResponse(user);
    }
    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponse>> RefreshToken(RefreshTokenRequest request)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
        {
            return BadRequest(new { message = "invalid access token.'" });
        }
        var userName = principal.Identity?.Name;
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            return BadRequest(new { message = "User not found." });
        }
        var storedRefreshToken = await _dbContext.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.UserId == user.Id && rt.Token == request.RefreshToken);
        if (storedRefreshToken == null || storedRefreshToken.ExpiryDate <= DateTime.UtcNow)
        {
            return BadRequest(new { message = "Invalid or expired refresh token." });
        }
        return await GenerateAuthResponse(user);
    }
    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        var username = User.Identity?.Name;
        if (username == null)
        {
            return BadRequest(new { message = "Invalid user." });
        }
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            return BadRequest(new { message = "User not found." });
        }
        await _signInManager.SignOutAsync();
        return Ok(new { message = "Logged out successfully." });
    }
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserInfo>> GetCurrentUser()
    {
        var username = User.Identity?.Name;
        if (username == null)
        {
            return BadRequest(new { message = "Invalid user." });
        }
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            return BadRequest(new { message = "User not found." });
        }
        return Ok(new UserInfo
        {
            Id = int.Parse(user.Id),
            UserName = user.UserName,
            Email = user.Email
        });
    }

    private async Task<AuthResponse> GenerateAuthResponse(AppUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(roles => new Claim(ClaimTypes.Role, roles))); 

        var accessToken = _tokenService.GenerateAccessToken(claims);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = HashToken(refreshToken),
            IssuedAt = DateTimeOffset.UtcNow.AddDays(
                int.Parse(_configuration["Jwt:RefreshTokenValidityInDays"]!)),
            UserAgent = Request.Headers.UserAgent.ToString(),
            Ip = HttpContext.Connection.RemoteIpAddress?.ToString()
        };
        _dbContext.Set<RefreshToken>().Add(refreshTokenEntity);
        await _dbContext.SaveChangesAsync();
        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["Jwt:AccessTokenValidityInMinutes"]!)),
            User = new UserInfo
            {
                Id = int.Parse(user.Id),
                Email = user.Email,
                UserName = user.UserName,
            }
        }; }

        private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
     
}

internal class RefreshToken
{
    public string UserId { get; set; }
    public string TokenHash { get; set; }
    public DateTimeOffset IssuedAt { get; set; }
    public string UserAgent { get; set; }
    public string Ip { get; set; }
    public string Token { get; internal set; }
    public DateTime ExpiryDate { get; internal set; }
}