namespace PhonebookAPI.Models
{
  public class FirstName
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Relazione: un nome può essere usato da molti utenti
    public ICollection<User> Users { get; set; } = new List<User>();

    // Relazione: un nome può essere in molti contatti
    public ICollection<PhoneBookEntry> PhoneBookEntries { get; set; } = new List<PhoneBookEntry>();
  }
}
