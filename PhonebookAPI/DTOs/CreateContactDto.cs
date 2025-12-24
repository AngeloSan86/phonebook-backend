namespace PhonebookAPI.DTOs
{
  public class CreateContactDto
  {
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
  }
}
