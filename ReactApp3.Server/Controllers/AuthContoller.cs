using System;
using Microsoft.AspNetCore.Mvc;
using LabelingTool.BLL.DTOs;
using LabelingTool.DAL.Interfaces;
using LabelingTool.BLL.Interfaces;
using Model;
using Microsoft.AspNetCore.Authorization;

namespace Smart_Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IUserRepository _userRepository;
		private readonly ITokenService _tokenService;

		public AuthController(IUserRepository userRepository, ITokenService tokenService)
		{
			_userRepository = userRepository;
			_tokenService = tokenService;
		}
		[HttpPost("register")]
		public ActionResult<AuthResponse> Register([FromBody] RegisterRequestDTO request)
		{
			if (request == null)
				return BadRequest("Request body is required.");

			var errors = RegisterValidator.Validate(request);
			if (errors.Count > 0)
			{
				return BadRequest(new { errors });
			}
			if (_userRepository.EmailExists(request.Email))
			{
				return BadRequest(new
				{
					errors = new[] { "Email is already in use." }
				});
			}

			// without hahing yet
			var user = new AppUser
			{
				Name = request.UserName,     
				Email = request.Email,
				PasswordHash = request.Password // TODO: hash
			};

			int newId = _userRepository.Create(user);
			user.Id = newId;

			var accessToken = _tokenService.CreateToken(user);
			var refreshToken = _tokenService.GenerateRefreshToken();
			var expiresAt = DateTime.UtcNow.AddMinutes(30);

			_userRepository.SaveRefreshToken(user.Id, refreshToken, expiresAt);

			var response = new AuthResponse
			{
				AccessToken = accessToken,
				RefreshToken = refreshToken,
				ExpiresAt = expiresAt,
				User = new UserInfo
				{
					Id = user.Id,
					Email = user.Email,
					Name = user.Name
				}
			};

			return Ok(response);
		}

		[HttpPost("login")]
		public ActionResult<AuthResponse> Login([FromBody] LoginRequestDTO request)
		{
			if (request == null ||
				string.IsNullOrWhiteSpace(request.Email) ||
				string.IsNullOrWhiteSpace(request.Password))
			{
				return BadRequest("Email and password are required.");
			}

			var user = _userRepository.GetByEmail(request.Email);
			if (user == null)
			{
				return Unauthorized("Invalid email or password.");
			}

			if (user.PasswordHash != request.Password)
			{
				return Unauthorized("Invalid email or password.");
			}

			var accessToken = _tokenService.CreateToken(user);
			var refreshToken = _tokenService.GenerateRefreshToken();
			var expiresAt = DateTime.UtcNow.AddMinutes(30);

			_userRepository.SaveRefreshToken(user.Id, refreshToken, expiresAt);


			var response = new AuthResponse
			{
				AccessToken = accessToken,
				RefreshToken = refreshToken,
				ExpiresAt = expiresAt,
				User = new UserInfo
				{
					Id = user.Id,
					Email = user.Email,
					Name = user.Name
				}
			};

			return Ok(response);
		}
		[HttpPost("refresh")]
		public ActionResult<AuthResponse> Refresh([FromBody] RefreshTokenRequest request)
		{
			if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
				return BadRequest("Refresh token is required.");

			var user = _userRepository.GetByRefreshToken(request.RefreshToken);
			if (user == null)
			{
				return Unauthorized("Invalid or expired refresh token.");
			}

			var newAccessToken = _tokenService.CreateToken(user);
			var newRefreshToken = _tokenService.GenerateRefreshToken();
			var newExpiresAt = DateTime.UtcNow.AddMinutes(30);

			_userRepository.SaveRefreshToken(user.Id, newRefreshToken, newExpiresAt);

			var response = new AuthResponse
			{
				AccessToken = newAccessToken,
				RefreshToken = newRefreshToken,
				ExpiresAt = newExpiresAt,
				User = new UserInfo
				{
					Id = user.Id,
					Email = user.Email,
					Name = user.Name
				}
			};

			return Ok(response);
		}
		[Authorize]
		[HttpGet("me")]
		public ActionResult<UserInfo> Me()
		{
			try
			{
				//here we are getting user id from token xd
				var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

				if (userIdClaim == null)
					return Unauthorized("Missing user id claim. Token invalid.");

				int userId = int.Parse(userIdClaim.Value);

				var user = _userRepository.GetById(userId);

				if (user == null)
					return Unauthorized("User not found.");

				var response = new UserInfo
				{
					Id = user.Id,
					Email = user.Email,
					Name = user.Name
				};

				return Ok(response);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error in /auth/me: " + ex.Message);
			}
		}
	}
}
