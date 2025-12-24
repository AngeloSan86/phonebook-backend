namespace PhonebookAPI.Models
{
  public class User
  {
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    // Foreign Keys
    public int FirstNameId { get; set; }
    public int LastNameId { get; set; }

    // URLs immagini
    public string ProfileImageUrl { get; set; } = "https://res.cloudinary.com/demo/image/upload/v1/default-profile.png";
    public string BackgroundImageUrl { get; set; } = "https://res.cloudinary.com/demo/image/upload/v1/default-background.jpg";

    // Timestamp
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Relazioni di navigazione
    public FirstName FirstName { get; set; } = null!;
    public LastName LastName { get; set; } = null!;

    // Un utente ha molti contatti in rubrica
    public ICollection<PhoneBookEntry> PhoneBookEntries { get; set; } = new List<PhoneBookEntry>();
  }
}
