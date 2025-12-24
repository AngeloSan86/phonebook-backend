
namespace PhonebookAPI.DTOs
{
  public class AuthResponseDto
  {
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ProfileImageUrl { get; set; } = string.Empty;
    public string BackgroundImageUrl { get; set; } = string.Empty;
  }
}
