using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Glossary.Application.Dtos;
using Glossary.Infrastructure.Auth;
using Glossary.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Glossary.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
    {
    private readonly GlossaryDbContext _db;
    private readonly IConfiguration _config;
    private readonly PasswordHasher<AppUser> _hasher = new();

    public AuthController(GlossaryDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(AuthRequest request, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Username == request.Username, ct);
        if (user is null)
            return Unauthorized(new { error = "Invalid credentials." });

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
            return Unauthorized(new { error = "Invalid credentials." });

        var token = CreateJwt(user);
        return Ok(new AuthResponse(token));
    }

    private string CreateJwt(AppUser user)
    {
        var jwt = _config.GetSection("Jwt");
        var issuer = jwt["Issuer"]!;
        var audience = jwt["Audience"]!;
        var key = jwt["Key"]!;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

