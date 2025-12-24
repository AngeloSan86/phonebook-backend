using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhonebookAPI.Data;
using PhonebookAPI.DTOs;
using PhonebookAPI.Models;
using System.Security.Claims;

namespace PhonebookAPI.Controllers
{
  [Authorize] // Richiede autenticazione JWT
  [Route("api/[controller]")]
  [ApiController]
  public class ContactsController : ControllerBase
  {
    private readonly ApplicationDbContext _context;

    public ContactsController(ApplicationDbContext context)
    {
      _context = context;
    }

    // GET: api/contacts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContactDto>>> GetContacts([FromQuery] string? sortBy = "lastname")
    {
      // Ottieni UserId dal token JWT
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (userIdClaim == null)
        return Unauthorized();

      int userId = int.Parse(userIdClaim);

      // Ottieni tutti i contatti dell'utente
      var contacts = await _context.PhoneBookEntries
          .Where(c => c.UserId == userId)
          .Include(c => c.FirstName)
          .Include(c => c.LastName)
          .ToListAsync();

      // Converti in DTO
      var contactDtos = contacts.Select(c => new ContactDto
      {
        Id = c.Id,
        FirstName = c.FirstName.Name,
        LastName = c.LastName.Surname,
        PhoneNumber = c.PhoneNumber
      }).ToList();

      // Ordina
      if (sortBy?.ToLower() == "firstname")
      {
        contactDtos = contactDtos
            .OrderBy(c => c.FirstName)
            .ThenBy(c => c.LastName)
            .ToList();
      }
      else // default: lastname
      {
        contactDtos = contactDtos
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToList();
      }

      return Ok(contactDtos);
    }

    // GET: api/contacts/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ContactDto>> GetContact(int id)
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (userIdClaim == null)
        return Unauthorized();

      int userId = int.Parse(userIdClaim);

      var contact = await _context.PhoneBookEntries
          .Include(c => c.FirstName)
          .Include(c => c.LastName)
          .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

      if (contact == null)
        return NotFound();

      return Ok(new ContactDto
      {
        Id = contact.Id,
        FirstName = contact.FirstName.Name,
        LastName = contact.LastName.Surname,
        PhoneNumber = contact.PhoneNumber
      });
    }

    // POST: api/contacts
    [HttpPost]
    public async Task<ActionResult<ContactDto>> CreateContact(CreateContactDto dto)
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (userIdClaim == null)
        return Unauthorized();

      int userId = int.Parse(userIdClaim);

      // Ottieni o crea FirstName
      var firstName = await _context.FirstNames
          .FirstOrDefaultAsync(f => f.Name == dto.FirstName);

      if (firstName == null)
      {
        firstName = new FirstName { Name = dto.FirstName };
        _context.FirstNames.Add(firstName);
        await _context.SaveChangesAsync();
      }

      // Ottieni o crea LastName
      var lastName = await _context.LastNames
          .FirstOrDefaultAsync(l => l.Surname == dto.LastName);

      if (lastName == null)
      {
        lastName = new LastName { Surname = dto.LastName };
        _context.LastNames.Add(lastName);
        await _context.SaveChangesAsync();
      }

      // Crea il contatto
      var contact = new PhoneBookEntry
      {
        UserId = userId,
        FirstNameId = firstName.Id,
        LastNameId = lastName.Id,
        PhoneNumber = dto.PhoneNumber
      };

      _context.PhoneBookEntries.Add(contact);
      await _context.SaveChangesAsync();

      // Carica le relazioni per la risposta
      await _context.Entry(contact).Reference(c => c.FirstName).LoadAsync();
      await _context.Entry(contact).Reference(c => c.LastName).LoadAsync();

      var contactDto = new ContactDto
      {
        Id = contact.Id,
        FirstName = contact.FirstName.Name,
        LastName = contact.LastName.Surname,
        PhoneNumber = contact.PhoneNumber
      };

      return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contactDto);
    }

    // PUT: api/contacts/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateContact(int id, CreateContactDto dto)
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (userIdClaim == null)
        return Unauthorized();

      int userId = int.Parse(userIdClaim);

      var contact = await _context.PhoneBookEntries
          .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

      if (contact == null)
        return NotFound();

      // Ottieni o crea FirstName
      var firstName = await _context.FirstNames
          .FirstOrDefaultAsync(f => f.Name == dto.FirstName);

      if (firstName == null)
      {
        firstName = new FirstName { Name = dto.FirstName };
        _context.FirstNames.Add(firstName);
        await _context.SaveChangesAsync();
      }

      // Ottieni o crea LastName
      var lastName = await _context.LastNames
          .FirstOrDefaultAsync(l => l.Surname == dto.LastName);

      if (lastName == null)
      {
        lastName = new LastName { Surname = dto.LastName };
        _context.LastNames.Add(lastName);
        await _context.SaveChangesAsync();
      }

      // Aggiorna il contatto
      contact.FirstNameId = firstName.Id;
      contact.LastNameId = lastName.Id;
      contact.PhoneNumber = dto.PhoneNumber;

      await _context.SaveChangesAsync();

      return NoContent();
    }

    // DELETE: api/contacts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContact(int id)
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (userIdClaim == null)
        return Unauthorized();

      int userId = int.Parse(userIdClaim);

      var contact = await _context.PhoneBookEntries
          .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

      if (contact == null)
        return NotFound();

      _context.PhoneBookEntries.Remove(contact);
      await _context.SaveChangesAsync();

      return NoContent();
    }
  }
}
