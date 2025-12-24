namespace PhonebookAPI.DTOs
{
  public class UpdateUserDto
  {
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Password { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? BackgroundImageUrl { get; set; }
  }
}
