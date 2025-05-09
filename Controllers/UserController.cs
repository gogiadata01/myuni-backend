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
                user.Name,
                user.Img,
                user.Coin,
                user.RemainingTime
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
            Token = newUserDto.Token
        };

        dbContext.MyUser.Add(newUser);
        dbContext.SaveChanges();

        return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
    }

    // ეს არის ავტორიზაცია ქულის მომატების გარეშე იყოს აქ ცოტანახი

[HttpPost("signin")]
public IActionResult SignIn([FromBody] UserSignInDto loginDto)
{
    if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
    {
        return BadRequest("Email and Password are required.");
    }

    try
    {
        var user = dbContext.MyUser.FirstOrDefault(u => u.Email == loginDto.Email);
        if (user == null || !VerifyPassword(loginDto.Password, user.Password))
        {
            return Unauthorized("Invalid email or password.");
        }

        var tokenString = GenerateJwtToken(user);

        return Ok(new
        {
            Token = tokenString,
        });
    }
    catch (Exception ex)
    {
        return StatusCode(500, "An error occurred. Please try again later.");
    }
}



//  ეს არი ავტორიზაცია ქულის მომატებით

// [HttpPost("signin")]
// public async Task<IActionResult> SignIn([FromBody] UserSignInDto userSignInDto)
// {
//     var user = await dbContext.MyUser.FirstOrDefaultAsync(u =>
//         u.Email == userSignInDto.Email && u.Password == HashPassword(userSignInDto.Password));

//     if (user == null)
//         return NotFound(new { message = "Invalid email or password" });

//     bool earnedCoins = false;

//     if (user.LastCoinAwardTime == null || user.LastCoinAwardTime.Value.AddHours(24) <= DateTime.UtcNow)
//     {
//         user.Coin += 3;
//         earnedCoins = true;
//         user.LastCoinAwardTime = DateTime.UtcNow;  // ✅ only update when coins are added
//     }

//     user.LastLogin = DateTime.UtcNow;  // Optional: track pure login time separately

//     await dbContext.SaveChangesAsync();

//     var token = GenerateJwtToken(user);

//     return Ok(new
//     {
//         userId = user.Id,
//         userName = user.Name,
//         email = user.Email,
//         password = user.Password,
//         type = user.Type,
//         img = user.Img,
//         coin = user.Coin,
//         remainingTime = user.RemainingTime,
//         token = token,
//         earnedCoins = earnedCoins
//     });
// }





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
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) // Add Id as the NameIdentifier claim
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: configuration["Jwt:Issuer"],
        audience: configuration["Jwt:Audience"],
        claims: claims,
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
    [HttpPut("update-coins")]
public IActionResult UpdateCoins([FromBody] UpdateCoinDto dto)
{
    if (dto.NewCoinValue < 0)
    {
        return BadRequest(new { message = "Coin value cannot be negative" });
    }

    // Fetch all users
    var allUsers = dbContext.MyUser.ToList();
    if (allUsers == null || !allUsers.Any())
    {
        return NotFound(new { message = "No users found." });
    }

    // Update the Coin for all users
    foreach (var user in allUsers)
    {
        user.Coin = dto.NewCoinValue;
    }

    // Save the changes to the database
    dbContext.SaveChanges();

    // Return success message
    return Ok(new { message = "Coin value updated for all users successfully." });
}
[HttpPost("UpdateRemainingTimeToAllUsers")]
public async Task<IActionResult> UpdateRemainingTimeToAllUsers()
{
    var users = await dbContext.MyUser.ToListAsync();

    if (users.Count == 0)
    {
        return NotFound("No users found.");
    }

    foreach (var user in users)
    {
        user.RemainingTime = 0;
    }

    await dbContext.SaveChangesAsync();
    
    return Ok("Remaining time updated to 0 for all users.");
}

// [HttpPost("AddQuizCompletion")]
// public async Task<IActionResult> AddQuizCompletion([FromBody] QuizCompletionDto data)
// {
//     int userId = data.UserId;
//     string completedDate = data.CompletedDate?.Trim();

//     Console.WriteLine($"[DEBUG] Incoming userId: {userId}");
//     Console.WriteLine($"[DEBUG] Incoming completedDate: '{completedDate}'");

//     var officialQuizDates = new List<string>
//     {
//         "05/10 15:00", "05/14 18:00", "05/18 15:00",
//         "05/21 18:00", "05/24 15:00", "05/27 18:00", "05/31 15:00"
//     };

//     if (!officialQuizDates.Contains(completedDate))
//     {
//         Console.WriteLine($"[DEBUG] Date '{completedDate}' is NOT in official dates.");
//         return Ok(new { message = "არ ემთხვევა ოფიციალურ ქვიზის თარიღს." });
//     }
//     Console.WriteLine($"[DEBUG] Date '{completedDate}' matches an official date.");

//     var alreadyExists = await dbContext.UserQuizCompletions
//         .AnyAsync(q => q.UserId == userId && q.CompletedDate == completedDate);

//     Console.WriteLine($"[DEBUG] Already exists: {alreadyExists}");

//     if (!alreadyExists)
//     {
//         dbContext.UserQuizCompletions.Add(new UserQuizCompletion
//         {
//             UserId = userId,
//             CompletedDate = completedDate
//         });
//         await dbContext.SaveChangesAsync();
//         Console.WriteLine($"[DEBUG] Added quiz completion to DB.");
//     }

//     var userCompletions = await dbContext.UserQuizCompletions
//         .Where(q => q.UserId == userId)
//         .Select(q => q.CompletedDate.Trim())
//         .Distinct()
//         .ToListAsync();

//     Console.WriteLine($"[DEBUG] User completions: {string.Join(", ", userCompletions)}");

//     var matchedCount = userCompletions.Count(date => officialQuizDates.Contains(date));
//     Console.WriteLine($"[DEBUG] matchedCount: {matchedCount}");

//     if (matchedCount == 7)
//     {
//         var user = await dbContext.MyUser
//             .Where(u => u.Id == userId)
//             .FirstOrDefaultAsync();

//         if (user == null)
//         {
//             Console.WriteLine("[DEBUG] User not found.");
//             return Ok(new { message = "მომხმარებელი ვერ მოიძებნა." });
//         }

//         user.Coin += 20;

//         // ✅ Force EF to mark the Coin property as modified:
//         dbContext.Entry(user).Property(u => u.Coin).IsModified = true;

//         var saveResult = await dbContext.SaveChangesAsync();
//         Console.WriteLine($"[DEBUG] Coins added. SaveChangesAsync result: {saveResult}. New coin value: {user.Coin}");

//         return Ok(new { message = "გილოცავ! დაემატა 20 ქოინი." });
//     }

//     return Ok(new { message = "ქვიზი შენახულია, ქოინი ჯერ არ დაგემატა." });
// }
[HttpPost("AddQuizCompletion")]
public async Task<IActionResult> AddQuizCompletion([FromBody] QuizCompletionDto data)
{
    if (data == null || data.UserId <= 0 || string.IsNullOrWhiteSpace(data.CompletedDate))
    {
        return BadRequest(new { message = "მონაცემები არასწორია." });
    }

    int userId = data.UserId;
    string completedDate = data.CompletedDate.Trim();

    Console.WriteLine($"[DEBUG] Incoming userId: {userId}");
    Console.WriteLine($"[DEBUG] Incoming completedDate: '{completedDate}'");

    var officialQuizDates = new List<string>
    {
        "05/10 15:00", "05/14 18:00", "05/18 15:00",
        "05/21 18:00", "05/24 15:00", "05/27 18:00", "05/31 15:00"
    };

    if (!officialQuizDates.Contains(completedDate))
    {
        Console.WriteLine($"[DEBUG] Date '{completedDate}' is NOT in official dates.");
        return Ok(new { message = "არ ემთხვევა ოფიციალურ ქვიზის თარიღს." });
    }

    Console.WriteLine($"[DEBUG] Date '{completedDate}' matches an official date.");

    var alreadyExists = await dbContext.UserQuizCompletions
        .AnyAsync(q => q.UserId == userId && q.CompletedDate == completedDate);

    Console.WriteLine($"[DEBUG] Already exists: {alreadyExists}");

    if (!alreadyExists)
    {
        dbContext.UserQuizCompletions.Add(new UserQuizCompletion
        {
            UserId = userId,
            CompletedDate = completedDate
        });

        await dbContext.SaveChangesAsync();
        Console.WriteLine($"[DEBUG] Added quiz completion to DB.");
    }

    // Fetch completions again in case it was just added
    var userCompletions = await dbContext.UserQuizCompletions
        .Where(q => q.UserId == userId)
        .Select(q => q.CompletedDate.Trim())
        .Distinct()
        .ToListAsync();

    Console.WriteLine($"[DEBUG] User completions: {string.Join(", ", userCompletions)}");

    var matchedCount = userCompletions.Count(date => officialQuizDates.Contains(date));
    Console.WriteLine($"[DEBUG] matchedCount: {matchedCount}");

    if (matchedCount == 7)
    {
        var user = await dbContext.MyUser.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            Console.WriteLine("[DEBUG] User not found.");
            return Ok(new { message = "მომხმარებელი ვერ მოიძებნა." });
        }

        Console.WriteLine($"[DEBUG] Current coin value: {user.Coin}");

        // Optional: prevent double reward by checking if already received
        if (user.Coin >= 20)
        {
            Console.WriteLine("[DEBUG] User already received reward.");
            return Ok(new { message = "ქოინი უკვე დამატებულია." });
        }

        user.Coin += 20;

        dbContext.Entry(user).Property(u => u.Coin).IsModified = true;

        var saveResult = await dbContext.SaveChangesAsync();
        Console.WriteLine($"[DEBUG] SaveChangesAsync result: {saveResult}, New coin: {user.Coin}");

        if (saveResult > 0)
        {
            return Ok(new { message = "გილოცავ! დაემატა 20 ქოინი." });
        }
        else
        {
            return Ok(new { message = "ქოინის დამატება ვერ მოხერხდა." });
        }
    }

    return Ok(new { message = "ქვიზი შენახულია, ქოინი ჯერ არ დაგემატა." });
}





[HttpDelete("DeleteAllQuizCompletions")]
public async Task<IActionResult> DeleteAllQuizCompletions(int userId)
{
    var entries = await dbContext.UserQuizCompletions
        .Where(q => q.UserId == userId)
        .ToListAsync();

    if (entries == null || entries.Count == 0)
    {
        return NotFound(new { message = "No quiz completions found for this user." });
    }

    dbContext.UserQuizCompletions.RemoveRange(entries);
    await dbContext.SaveChangesAsync();

    return Ok(new { message = "All quiz completions deleted for user." });
}




[HttpPut("update-remaining-time/{userId}")]
public IActionResult UpdateRemainingTime(int userId, [FromBody] int additionalTime)
{
    Console.WriteLine($"Received request: userId={userId}, additionalTime={additionalTime}");

    if (additionalTime == 0) {
        return BadRequest("Additional time cannot be zero.");
    }

    var user = dbContext.MyUser.FirstOrDefault(u => u.Id == userId);
    if (user == null)
    {
        return NotFound("User not found.");
    }

    user.RemainingTime += additionalTime;
    dbContext.SaveChanges();

    return Ok(new
    {
        Message = "Remaining time updated successfully.",
        UpdatedRemainingTime = user.RemainingTime
    });
}


[HttpPost("recover-password")]
public IActionResult RecoverPassword([FromBody] UserRecoverPasswordDto recoverPasswordDto)
{
    // 1. Check if the email exists in the database
    var user = dbContext.MyUser.FirstOrDefault(u => u.Email == recoverPasswordDto.Email);
    if (user == null)
    {
        return NotFound(new { Message = "User with this email does not exist." });
    }

    // 2. Hash the new password (SHA256 or other algorithm)
    string hashedPassword = HashPassword(recoverPasswordDto.NewPassword);

    // 3. Update the user's password in the database
    user.Password = hashedPassword;
    dbContext.SaveChanges();

    // Return a success message
    return Ok(new { Message = "Password has been successfully updated." });
}

        // Helper function to hash the password using SHA256


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
    [HttpGet("check-quiz-restriction/{userId}")]
public IActionResult CheckQuizRestriction(int userId)
{
    var user = dbContext.MyUser.FirstOrDefault(u => u.Id == userId);
    if (user == null)
    {
        return NotFound("User not found.");
    }

    // Check if the user has a last quiz attempt time
    if (user.LastQuizAttempt.HasValue)
    {
        var timeSinceLastAttempt = DateTime.UtcNow - user.LastQuizAttempt.Value;

        if (timeSinceLastAttempt < TimeSpan.FromMinutes(15))
        {
            var timeLeft = TimeSpan.FromMinutes(15) - timeSinceLastAttempt;
            return Ok(new
            {
                CanStartQuiz = false,
                TimeUntilNextAttempt = timeLeft.TotalSeconds
            });
        }
    }

    // User can start the quiz
    return Ok(new
    {
        CanStartQuiz = true,
        TimeUntilNextAttempt = 0
    });
}
[HttpGet("quiz-history/{userId}")]
public async Task<IActionResult> GetQuizHistory(int userId)
{
    var user = await dbContext.MyUser
        .AsNoTracking()
        .Include(u => u.Quizes)
            .ThenInclude(q => q.QuizQuestions)
                .ThenInclude(qq => qq.BadAnswers)
        .FirstOrDefaultAsync(u => u.Id == userId);

    if (user == null)
    {
        return NotFound("User not found.");
    }

    var quizHistoryDtos = user.Quizes.Select(q => new
    {
        q.time,
        QuizQuestions = q.QuizQuestions.Select(qq => new
        {
            qq.question,
            qq.correctanswer,
            qq.UserAnswer,
            qq.img,
            BadAnswers = qq.BadAnswers.Select(b => new  { b.badanswer})
            .ToList()
        }).ToList()
    }).ToList();

    return Ok(quizHistoryDtos);
}
[HttpDelete("quiz-history/{userId}")]
public IActionResult DeleteUserQuizHistory(int userId)
{
    var user = dbContext.MyUser
        .Include(u => u.Quizes)
            .ThenInclude(q => q.QuizQuestions)
                .ThenInclude(qq => qq.BadAnswers)
        .FirstOrDefault(u => u.Id == userId);

    if (user == null)
    {
        return NotFound("User not found.");
    }

    // Remove quiz history directly from user
    user.Quizes.Clear();

    dbContext.SaveChanges();
    return NoContent();
}



[HttpPost("end-quiz/{userId}")]
public IActionResult EndQuiz(int userId, [FromBody] int correctAnswers)
{
    
    var user = dbContext.MyUser.FirstOrDefault(u => u.Id == userId);
    if (user == null)
    {
        return NotFound("User not found.");
    }

    // Update coins
    user.Coin += correctAnswers;

    // Update last quiz attempt time
    user.LastQuizAttempt = DateTime.UtcNow;

    dbContext.SaveChanges();

    return Ok(new
    {
        Message = "Quiz ended successfully.",
        CoinsEarned = correctAnswers,
        TotalCoins = user.Coin
    });
}

}


