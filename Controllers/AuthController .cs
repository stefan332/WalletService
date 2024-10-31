using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BAL.Models.Requests;
using BAL.Contratcs.MyToken;
using Shared;
using BAL.Configuration;
using Microsoft.IdentityModel.Tokens;
using Data;
using Microsoft.Extensions.Options;
using Data.Domain;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IMyTokenService _tokenService;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly IStore _store;
    private readonly JwtConfig _jwtConfig;
    private readonly ILogger<AuthController> _logger;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AuthController(UserManager<IdentityUser> userManager, IMyTokenService tokenService, RoleManager<IdentityRole> roleManager,
                                    TokenValidationParameters tokenValidationParameters,
                                    IOptionsMonitor<JwtConfig> optionsMonitor,
                                    IStore store,
                                    ILogger<AuthController> logger,
                                    SignInManager<IdentityUser> signInManager)
    {
        _logger = logger;
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenValidationParameters = tokenValidationParameters;
        _jwtConfig = optionsMonitor.CurrentValue;
        _store = store;
        _signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] ReqisterDto registerDto)
    {
        var error = new List<string>();
        if (ModelState.IsValid)
        {

            var existingUserEmail = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUserEmail is not null)
            {
                error.Add("email is allready in use");
                return BadRequest("email is allready in use");
            }
            var existingUserName = await _userManager.FindByNameAsync(registerDto.Username);
            if (existingUserName is not null)
            {
                error.Add("User name is allready in use");
                return BadRequest("User name is allready in use");
            }
            var newUser = new IdentityUser() { Email = registerDto.Email, UserName = registerDto.Username, };
            var isCreated = await _userManager.CreateAsync(newUser, registerDto.Password);
            if (isCreated.Succeeded)
            {
                // We need to add the user to a role
                //we need to check if that role exists
                var roleExists = await _roleManager.RoleExistsAsync(StaticRoles.Player);
                if (!roleExists)
                {
                    var createdRole = await _roleManager.CreateAsync(new IdentityRole(StaticRoles.Player));
                    if (createdRole.Succeeded)
                        await _userManager.AddToRoleAsync(newUser, StaticRoles.Player);
                }

                await _userManager.AddToRoleAsync(newUser, StaticRoles.Player);
                var jwtToken = await _tokenService.GenerateJwtToken(newUser);
                await _signInManager.SignInAsync(newUser, false);
                return Ok(jwtToken);
            }
            else
            {
                var Errors = new List<string>();
                Errors = isCreated.Errors.Select(x => x.Description).ToList();
                return BadRequest(Errors);

            }

        }
        error.Add("Invalid payload");
        return BadRequest("Invalid payload");
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LogIn([FromBody] UserLoginRequest user)
    {
        var error = new List<string>();
        if (ModelState.IsValid)
        {
            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser == null)
            {

                error.Add("Invalid login request");
                return BadRequest(error);

            }
            var isCorrect = await _userManager.CheckPasswordAsync(existingUser, user.Password);
            if (!isCorrect)
            {
                error.Add("Invalid login request");
                return BadRequest(error);
            }

            var jwtToken = await _tokenService.GenerateJwtToken(existingUser, true);
            await _signInManager.SignInAsync(existingUser, false);
            return Ok(jwtToken);
        }

        error.Add("Invalid payload");
        return BadRequest(error);
    }


    [HttpPost]
    [Route("refreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
    {
        var error = new List<string>();
        if (ModelState.IsValid)
        {
            var result = await _tokenService.VerifyAndGenerateToken(tokenRequest);
            if (result == null)
            {
                error.Add("Invalid Token");
                return BadRequest(error);

            }
            return Ok(result);
        }
        error.Add("Invalid Payload");
        return BadRequest(error);
    }

    [HttpPut]
    [Route("logout")]
    public async Task<IActionResult> LogOut(string currentUserId)
    {
        var currentUser = await _userManager.FindByIdAsync(currentUserId);
        if (currentUser is not null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation($"The User {currentUser.UserName} Logout successfuly");
            var rTokens = _store.Query<RefreshToken>().Where(x => x.UserId == currentUserId);

            foreach (var item in rTokens)
            {
                item.IsRevorked = true;
                item.IsUsed = false;
            }
            foreach (var token in rTokens)
            {
                _store.Update(token);
            }

            await _store.SaveChangesAsync();

            return Ok(true);
        }
        else
        {
            var error = new List<string>();
            error.Add("Current User is missing");
            return BadRequest(error);
        }


    }

}
