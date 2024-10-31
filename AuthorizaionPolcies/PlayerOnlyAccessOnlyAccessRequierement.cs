using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AuthorizaionPolcies
{
    public class PlayerOnlyAccessOnlyAccessRequierement : IAuthorizationRequirement
    { 

    }

    public class PlayerOnlyAccess : AuthorizationHandler<PlayerOnlyAccessOnlyAccessRequierement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PlayerOnlyAccessOnlyAccessRequierement requirement)
        {
            var roles = context.User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(c => c.Value);
            if (roles.Any(role => role.Equals("Player", StringComparison.OrdinalIgnoreCase)))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail(); // Explicitly fail if the user does not have the correct role
            }

            return Task.CompletedTask;
        }
    }
}