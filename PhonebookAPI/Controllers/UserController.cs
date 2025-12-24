using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhonebookAPI.Data;
using PhonebookAPI.DTOs;
using PhonebookAPI.Models;
using System.Security.Claims;

namespace PhonebookAPI.Controllers
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class UserController : ControllerBase
  {
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
      _context = context;
    }

    // GET: api/user/profile
    [HttpGet("profile")]
    public async Task<ActionResult<AuthResponseDto>> GetProfile()
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (userIdClaim == null)
        return Unauthorized();

      int userId = int.Parse(userIdClaim);

      var user = await _context.Users
          .Include(u => u.FirstName)
          .Include(u => u.LastName)
          .FirstOrDefaultAsync(u => u.Id == userId);

      if (user == null)
        return NotFound();

      return Ok(new AuthResponseDto
      {
        Token = "", // Non serve nel profilo
        UserId = user.Id,
        Email = user.Email,
        FirstName = user.FirstName.Name,
        LastName = user.LastName.Surname,
        ProfileImageUrl = user.ProfileImageUrl,
        BackgroundImageUrl = user.BackgroundImageUrl
      });
    }

    // PUT: api/user/profile
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateUserDto dto)
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (userIdClaim == null)
        return Unauthorized();

      int userId = int.Parse(userIdClaim);

      var user = await _context.Users
          .FirstOrDefaultAsync(u => u.Id == userId);

      if (user == null)
        return NotFound();

      // Aggiorna FirstName se fornito
      if (!string.IsNullOrEmpty(dto.FirstName))
      {
        var firstName = await _context.FirstNames
            .FirstOrDefaultAsync(f => f.Name == dto.FirstName);

        if (firstName == null)
        {
          firstName = new FirstName { Name = dto.FirstName };
          _context.FirstNames.Add(firstName);
          await _context.SaveChangesAsync();
        }

        user.FirstNameId = firstName.Id;
      }

      // Aggiorna LastName se fornito
      if (!string.IsNullOrEmpty(dto.LastName))
      {
        var lastName = await _context.LastNames
            .FirstOrDefaultAsync(l => l.Surname == dto.LastName);

        if (lastName == null)
        {
          lastName = new LastName { Surname = dto.LastName };
          _context.LastNames.Add(lastName);
          await _context.SaveChangesAsync();
        }

        user.LastNameId = lastName.Id;
      }

      // Aggiorna Password se fornita
      if (!string.IsNullOrEmpty(dto.Password))
      {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
      }

      // Aggiorna ProfileImageUrl se fornito
      if (!string.IsNullOrEmpty(dto.ProfileImageUrl))
      {
        user.ProfileImageUrl = dto.ProfileImageUrl;
      }

      // Aggiorna BackgroundImageUrl se fornito
      if (!string.IsNullOrEmpty(dto.BackgroundImageUrl))
      {
        user.BackgroundImageUrl = dto.BackgroundImageUrl;
      }

      user.UpdatedAt = DateTime.UtcNow;

      await _context.SaveChangesAsync();

      return NoContent();
    }

    // POST: api/user/reset-profile-image
    [HttpPost("reset-profile-image")]
    public async Task<IActionResult> ResetProfileImage()
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (userIdClaim == null)
        return Unauthorized();

      int userId = int.Parse(userIdClaim);

      var user = await _context.Users
          .FirstOrDefaultAsync(u => u.Id == userId);

      if (user == null)
        return NotFound();

      user.ProfileImageUrl = "https://res.cloudinary.com/demo/image/upload/v1/default-profile.png";
      user.UpdatedAt = DateTime.UtcNow;

      await _context.SaveChangesAsync();

      return Ok(new { profileImageUrl = user.ProfileImageUrl });
    }

    // POST: api/user/reset-background-image
    [HttpPost("reset-background-image")]
    public async Task<IActionResult> ResetBackgroundImage()
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (userIdClaim == null)
        return Unauthorized();

      int userId = int.Parse(userIdClaim);

      var user = await _context.Users
          .FirstOrDefaultAsync(u => u.Id == userId);

      if (user == null)
        return NotFound();

      user.BackgroundImageUrl = "https://res.cloudinary.com/demo/image/upload/v1/default-background.jpg";
      user.UpdatedAt = DateTime.UtcNow;

      await _context.SaveChangesAsync();

      return Ok(new { backgroundImageUrl = user.BackgroundImageUrl });
    }

    // DELETE: api/user/account
    [HttpDelete("account")]
    public async Task<IActionResult> DeleteAccount()
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (userIdClaim == null)
        return Unauthorized();

      int userId = int.Parse(userIdClaim);

      var user = await _context.Users
          .FirstOrDefaultAsync(u => u.Id == userId);

      if (user == null)
        return NotFound();

      // Grazie a CASCADE DELETE, i contatti vengono eliminati automaticamente
      _context.Users.Remove(user);
      await _context.SaveChangesAsync();

      return Ok(new { message = "Account eliminato con successo" });
    }
  }
}
