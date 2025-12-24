namespace PhonebookAPI.Models
{
  public class PhoneBookEntry
  {
    public int Id { get; set; }

    // Foreign Keys
    public int UserId { get; set; }
    public int FirstNameId { get; set; }
    public int LastNameId { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relazioni di navigazione
    public User User { get; set; } = null!;
    public FirstName FirstName { get; set; } = null!;
    public LastName LastName { get; set; } = null!;
  }
}
