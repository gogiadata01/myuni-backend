using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyUni.Data;
using MyUni.Models.Entities; // For User and other entities
using MyUni.Models; // For UserDto and UserSignInDto
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;
    private readonly IConfiguration configuration; // Configuration for JWT settings

    public UserController(ApplicationDbContext dbContext, IConfiguration configuration)
    {
        this.dbContext = dbContext;
        this.configuration = configuration;
    }

    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var allUsers = dbContext.MyUser
            .Select(user => new
            {
                user.Id,
                user.Email,
                user.Name
            })
            .ToList();

        if (!allUsers.Any())
        {
            return NotFound("No users found.");
        }

        return Ok(allUsers);
    }

    [HttpGet("{id}")]
    public IActionResult GetUserById(int id)
    {
        var user = dbContext.MyUser.Find(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] UserDto newUserDto)
    {
        if (newUserDto == null)
        {
            return BadRequest("User data is required.");
        }

        var existingUser = dbContext.MyUser.FirstOrDefault(u => u.Email == newUserDto.Email);
        if (existingUser != null)
        {
            return Conflict("A user with the same email already exists.");
        }

        string hashedPassword = HashPassword(newUserDto.Password);

        var newUser = new User
        {
            Name = newUserDto.Name,
            Email = newUserDto.Email,
            Password = hashedPassword,
            Type = newUserDto.Type,
            Img = newUserDto.Img,
            Coin = newUserDto.Coin,
            ResetToken = newUserDto.ResetToken,
        };

        dbContext.MyUser.Add(newUser);
        dbContext.SaveChanges();

        return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
    }

    [HttpPost("signin")]
    public IActionResult SignIn([FromBody] UserSignInDto loginDto)
    {
        if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
        {
            return BadRequest("Email and Password are required.");
        }

        var user = dbContext.MyUser.FirstOrDefault(u => u.Email == loginDto.Email);
        if (user == null || !VerifyPassword(loginDto.Password, user.Password))
        {
            return Unauthorized("Invalid email or password.");
        }

        var tokenString = GenerateJwtToken(user);

        return Ok(new { Token = tokenString, UserId = user.Id, UserName = user.Name });
    }

    private bool VerifyPassword(string inputPassword, string storedHashedPassword)
    {
        string hashedInputPassword = HashPassword(inputPassword);
        return hashedInputPassword == storedHashedPassword;
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Email), // Use email or username as the claim
            new Claim(ClaimTypes.Role, user.Type) // Assuming Type can represent roles like "Admin"
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Authorize(Roles = "Admin")] // Only admin users can access this action
    [HttpGet("admin-data")]
    public IActionResult GetAdminData()
    {
        // Logic to return admin-specific data
        return Ok(new { message = "This is admin data." });
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var user = dbContext.MyUser.Find(id);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        dbContext.MyUser.Remove(user);
        dbContext.SaveChanges();

        return Ok(new { message = "User deleted successfully" });
    }

    public class UpdateCoinDto
    {
        public int NewCoinValue { get; set; }
    }

    [HttpPut("{id}/coin")]
    public IActionResult UpdateCoin(int id, [FromBody] UpdateCoinDto dto)
    {
        var user = dbContext.MyUser.Find(id);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        user.Coin = dto.NewCoinValue;
        dbContext.SaveChanges();

        return Ok(new { message = "Coin value updated successfully", user });
    }
     [HttpPost("recover-password")]
        public IActionResult RecoverPassword(string email, string newPassword)
        {
            // 1. Check if the email exists in the database
            var user = dbContext.MyUser.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return NotFound(new { Message = "User with this email does not exist." });
            }

            // 2. Hash the new password (SHA256 or other algorithm)
            string hashedPassword = HashPassword(newPassword);

            // 3. Update the user's password in the database
            user.Password = hashedPassword;
            dbContext.SaveChanges();

            // Return a success message
            return Ok(new { Message = "Password has been successfully updated." });
        }

        // Helper function to hash the password using SHA256
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }


    [HttpPost("admin/login")] // New endpoint for admin login
    public IActionResult AdminLogin([FromBody] UserSignInDto adminLoginDto)
    {
        if (adminLoginDto == null || string.IsNullOrEmpty(adminLoginDto.Email) || string.IsNullOrEmpty(adminLoginDto.Password))
        {
            return BadRequest("Email and Password are required.");
        }

        // Check if the user is an admin in your database
        var user = dbContext.MyUser.FirstOrDefault(u => u.Email == adminLoginDto.Email);
        if (user == null || !VerifyPassword(adminLoginDto.Password, user.Password) || user.Type != "Admin")
        {
            return Unauthorized("Invalid admin credentials.");
        }

        var tokenString = GenerateJwtToken(user);

        return Ok(new { Token = tokenString, UserId = user.Id, UserName = user.Name });
    }
}


