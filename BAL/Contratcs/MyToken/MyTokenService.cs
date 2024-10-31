using BAL.Configuration;
using BAL.Factory;
using BAL.Models.Requests;
using BAL.Models.Response;
using Data;
using Data.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace BAL.Contratcs.MyToken
{
    public class MyTokenService : IMyTokenService
    {
        private readonly JwtConfig _jwtConfig;
        private readonly IStore _store;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ILogger<MyTokenService> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public MyTokenService(IStore store,
                            IOptionsMonitor<JwtConfig> options,
                            TokenValidationParameters tokenValidationParameters,
                            ILogger<MyTokenService> logger,
                            UserManager<IdentityUser> userManager,
                            RoleManager<IdentityRole> roleManager)
        {
            _jwtConfig = options.CurrentValue;
            _store = store;
            _tokenValidationParameters = tokenValidationParameters;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<AuthResult> GenerateJwtToken(IdentityUser user, bool fromLogIn = false)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
                var claims = await GetAllValidClaims(user, fromLogIn);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(6), // 5-10 
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = jwtTokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = jwtTokenHandler.WriteToken(token);
                var existingRefreshToken = await _store.Query<RefreshToken>()
                    .Where(x => x.UserId == user.Id).ToListAsync();
                _store.RemoveRange(existingRefreshToken);
                await _store.SaveChangesAsync();
                var refreshToken = token.ToEntity(user.Id);

                await _store.AddAsync(refreshToken);
                await _store.SaveChangesAsync();

                return new AuthResult()
                {
                    Token = jwtToken,
                    Success = true,
                    RefreshToken = refreshToken.Token
                };
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                throw new InvalidOperationException(exc.Message);
            }

        }
        private async Task<List<Claim>> GetAllValidClaims(IdentityUser user, bool fromLogin = false)
        {
            var claims = new List<Claim>
            {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),


            };
            if (fromLogin) claims.Add(new Claim("IsOnline", true.ToString()));


            // Getting the claims that we have assigned to the user
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            // Get the user role and add it to the claims
            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var userRole in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(userRole);

                if (role != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole));

                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (var roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }

            return claims;
        }
        public async Task<AuthResult> VerifyAndGenerateToken(TokenRequest tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtConfig.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    RequireExpirationTime = false,
                    ClockSkew = TimeSpan.Zero

                };
                // Validation 1 - Validation JWT token format
                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, tokenValidationParameters, out var validatedToken);

                // Validation 2 - Validate encryption alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if (result == false)
                    {
                        return null;
                    }
                }

                // Validation 3 - validate expiry date
                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

                if (expiryDate > DateTime.UtcNow)
                {
                    var error = new List<string>();
                    error.Add("Token has not yet expired");

                    return new AuthResult()
                    {
                        Success = false,
                        Error = error
                    };
                }

                // validation 4 - validate existence of the token
                var storedToken = await _store.Query<RefreshToken>().FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

                if (storedToken == null)
                {
                    var error = new List<string>();
                    error.Add("Token does not exist");
                    return new AuthResult()
                    {
                        Success = false,
                        Error = error
                    };
                }

                // Validation 5 - validate if used
                if (storedToken.IsUsed)
                {
                    var error = new List<string>();
                    error.Add("Token has been used");
                    return new AuthResult()
                    {
                        Success = false,
                        Error = error
                    };
                }

                // Validation 6 - validate if revoked
                if (storedToken.IsRevorked)
                {
                    var error = new List<string>();
                    error.Add("Token has been revoked");
                    return new AuthResult()
                    {
                        Success = false,
                        Error = error
                    };
                }

                // Validation 7 - validate the id
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                if (storedToken.JwtId != jti)
                {
                    var error = new List<string>();
                    error.Add("Token doesn't match");
                    return new AuthResult()
                    {
                        Success = false,
                        Error = error
                    };
                }

                // update current token 

                storedToken.IsUsed = true;
                _store.Update(storedToken);
                await _store.SaveChangesAsync();

                // Generate a new token
                var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);
                return await GenerateJwtToken(dbUser, true);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Lifetime validation failed. The token is expired."))
                {
                    var error = new List<string>();
                    error.Add("Token has expired please re-login");
                    return new AuthResult()
                    {
                        Success = false,
                        Error = error
                    };

                }
                else
                {
                    var error = new List<string>();
                    error.Add("Something went wrong");
                    return new AuthResult()
                    {
                        Success = false,
                        Error = error
                    };
                }
            }
        }
        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();

            return dateTimeVal;
        }
    }
}
