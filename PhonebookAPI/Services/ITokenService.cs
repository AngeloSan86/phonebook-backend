using PhonebookAPI.Models;

namespace PhonebookAPI.Services
{
  public interface ITokenService
  {
    string GenerateToken(User user);
  }
}
