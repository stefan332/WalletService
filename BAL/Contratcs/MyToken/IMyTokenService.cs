using BAL.Models.Requests;
using BAL.Models.Response;
using Microsoft.AspNetCore.Identity;

namespace BAL.Contratcs.MyToken
{
    public interface IMyTokenService
    {
        Task<AuthResult> GenerateJwtToken(IdentityUser user, bool fromLogIn = false);
        Task<AuthResult> VerifyAndGenerateToken(TokenRequest tokenRequest);
    }
}
