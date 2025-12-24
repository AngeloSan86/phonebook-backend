using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhonebookAPI.Data;
using PhonebookAPI.DTOs;
using PhonebookAPI.Models;
using PhonebookAPI.Services;

namespace PhonebookAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthController(ApplicationDbContext context, ITokenService tokenService)
    {
      _context = context;
      _tokenService = tokenService;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
      // 1. Verifica se l'email esiste già
      if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
      {
        return BadRequest(new { message = "Email già registrata" });
      }

      // 2. Ottieni o crea FirstName
      var firstName = await _context.FirstNames
          .FirstOrDefaultAsync(f => f.Name == dto.FirstName);

      if (firstName == null)
      {
        firstName = new FirstName { Name = dto.FirstName };
        _context.FirstNames.Add(firstName);
        await _context.SaveChangesAsync();
      }

      // 3. Ottieni o crea LastName
      var lastName = await _context.LastNames
          .FirstOrDefaultAsync(l => l.Surname == dto.LastName);

      if (lastName == null)
      {
        lastName = new LastName { Surname = dto.LastName };
        _context.LastNames.Add(lastName);
        await _context.SaveChangesAsync();
      }

      // 4. Hash della password
      var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

      // 5. Crea il nuovo utente
      var user = new User
      {
        Email = dto.Email,
        PasswordHash = passwordHash,
        FirstNameId = firstName.Id,
        LastNameId = lastName.Id
      };

      _context.Users.Add(user);
      await _context.SaveChangesAsync();

      // 6. Carica le relazioni per la risposta
      await _context.Entry(user).Reference(u => u.FirstName).LoadAsync();
      await _context.Entry(user).Reference(u => u.LastName).LoadAsync();

      // 7. Genera il token JWT
      var token = _tokenService.GenerateToken(user);

      // 8. Ritorna la risposta
      return Ok(new AuthResponseDto
      {
        Token = token,
        UserId = user.Id,
        Email = user.Email,
        FirstName = user.FirstName.Name,
        LastName = user.LastName.Surname,
        ProfileImageUrl = user.ProfileImageUrl,
        BackgroundImageUrl = user.BackgroundImageUrl
      });
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
      // 1. Trova l'utente per email
      var user = await _context.Users
          .Include(u => u.FirstName)
          .Include(u => u.LastName)
          .FirstOrDefaultAsync(u => u.Email == dto.Email);

      if (user == null)
      {
        return Unauthorized(new { message = "Email o password errati" });
      }

      // 2. Verifica la password
      if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
      {
        return Unauthorized(new { message = "Email o password errati" });
      }

      // 3. Genera il token JWT
      var token = _tokenService.GenerateToken(user);

      // 4. Ritorna la risposta
      return Ok(new AuthResponseDto
      {
        Token = token,
        UserId = user.Id,
        Email = user.Email,
        FirstName = user.FirstName.Name,
        LastName = user.LastName.Surname,
        ProfileImageUrl = user.ProfileImageUrl,
        BackgroundImageUrl = user.BackgroundImageUrl
      });
    }
  }
}
