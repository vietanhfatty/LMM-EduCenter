using Server.Models;

namespace Server.Services;

public interface IJwtService
{
    string CreateToken(AppUser user, IList<string> roles);
}
