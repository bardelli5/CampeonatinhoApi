using Microsoft.AspNetCore.Identity;

namespace CampeonatinhoApp.Interfaces
{
    public interface ITokenRepository
    {
        string CreateJwtToken(IdentityUser user, List<string> roles);
    }
}
