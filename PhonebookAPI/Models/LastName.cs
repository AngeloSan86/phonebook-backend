namespace PhonebookAPI.Models
{
  public class LastName
  {
    public int Id { get; set; }
    public string Surname { get; set; } = string.Empty;

    // Relazione: un cognome può essere usato da molti utenti
    public ICollection<User> Users { get; set; } = new List<User>();

    // Relazione: un cognome può essere in molti contatti
    public ICollection<PhoneBookEntry> PhoneBookEntries { get; set; } = new List<PhoneBookEntry>();
  }
}
