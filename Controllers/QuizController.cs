using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyUni.Data;
using MyUni.Models;
using MyUni.Models.Entities;
using System;
using System.Globalization;
using System.Linq;

namespace MyUni.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly Emailcs _emailService;
            private readonly ILogger<QuizController> _logger;  // Declare the logger


        public QuizController(ApplicationDbContext dbContext, Emailcs emailService,ILogger<QuizController> logger)
        {
            this.dbContext = dbContext;
            _emailService = emailService;
                    _logger = logger;  // Initialize the logger

        }

        // GET: api/Quiz
        [HttpGet]
        public IActionResult GetAllQuizzes()
        {
            var quizzes = dbContext.MyQuiz
                .Include(q => q.Questions)
                    .ThenInclude(qa => qa.IncorrectAnswers)
                .Include(q => q.BonusQuestion)
                    .ThenInclude(bq => bq.CorrectAnswers)
                .Include(q => q.BonusQuestion)
                    .ThenInclude(bq => bq.IncorrectAnswers)
                .ToList();

            return Ok(quizzes);
        }
//         [HttpGet("reminder")]
// public async Task<IActionResult> SendReminderForQuiz()
// {
//     try
//     {
//         var quizzes = await dbContext.MyQuiz
//             .Include(card => card.Questions)
//             .ThenInclude(incorrectAnswer => incorrectAnswer.IncorrectAnswers)
//             .ToListAsync();

//         var quiz = quizzes.FirstOrDefault();

//         if (quiz == null)
//         {
//             return NotFound(new { Message = "Quiz not found." });
//         }

//         Console.WriteLine($"Attempting to parse time: {quiz.Time}");

//         // Get the current year and construct the time string
//         var currentYear = DateTime.Now.Year;
//         var timeWithYear = $"{quiz.Time}/{currentYear}";

//         // Parse the quiz time
//         if (!DateTime.TryParseExact(timeWithYear, 
//                                      "MM/dd HH:mm/yyyy", 
//                                      CultureInfo.InvariantCulture, 
//                                      DateTimeStyles.None, 
//                                      out DateTime quizTime))
//         {
//             return BadRequest(new { Message = "Invalid quiz time format." });
//         }

//         Get the local time zone for Georgia
//         TimeZoneInfo localZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tbilisi");
//         DateTime quizTimeInLocalZone = TimeZoneInfo.ConvertTime(quizTime, localZone);

//         // Calculate the reminder time
//         var reminderTime = quizTimeInLocalZone.AddMinutes(-30);
//         var currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.Utc, localZone); // Get current local time

//         // Log times for debugging
//         Console.WriteLine($"Current Time (Local): {currentTime}");
//         Console.WriteLine($"Quiz Time (Local): {quizTimeInLocalZone}");
//         Console.WriteLine($"Reminder Time (Local): {reminderTime}");

//         // Check if it's time to send the reminder
//         if (currentTime >= reminderTime && currentTime <= quizTimeInLocalZone)
//         {
//             await _emailService.SendEmailToAllUsers(
//                 "Reminder: მოგესალმებით ქვიზი დაიწყება მალე.",
//                 "ქვიზის დაწყებამდე დარჩენილია 30 წუთი."
//             );

//             return Ok(new { Message = "Reminder emails have been sent to all users." });
//         }
//         else
//         {
//             return BadRequest(new { Message = "It's too early to send a reminder. Try again closer to the quiz time." });
//         }
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine(ex.Message);
//         return StatusCode(500, new { Message = "An error occurred while sending reminders." });
//     }
// }

// [HttpPost("send-email")]
// public async Task<IActionResult> SendCustomEmail([FromBody] EmailRequestDto emailRequest)
// {
//     // Validate that subject and body are not empty
//     if (string.IsNullOrEmpty(emailRequest.Subject) || string.IsNullOrEmpty(emailRequest.Body))
//     {
//         return BadRequest(new { Message = "Subject and Body are required." });
//     }

//     int maxRetries = 3;  // Maximum number of retries
//     int attempt = 0;
//     bool success = false;

//     try
//     {
//         // Check if the emails list is null or empty. If so, send to all users.
//         var sendToAllUsers = emailRequest.Emails == null || emailRequest.Emails.Count == 0;

//         // Retry logic
//         while (attempt < maxRetries && !success)
//         {
//             try
//             {
//                 // Corrected method call with proper arguments
//                 await _emailService.SendEmailsAsync(
//                     sendToAllUsers ? new List<string>() : emailRequest.Emails,  // List<string> emails
//                     emailRequest.Subject,  // string subject
//                     emailRequest.Body      // string body
//                 );
//                 success = true;  // If successful, exit the loop
//             }
//             catch (Exception ex)
//             {
//                 attempt++;
//                 // Log the error or notify that the attempt failed
//                 _logger.LogError($"Attempt {attempt} failed: {ex.Message}");

//                 if (attempt < maxRetries)
//                 {
//                     // Wait for a few seconds before retrying
//                     await Task.Delay(5000); // Delay for 5 seconds (adjust as needed)
//                 }
//             }
//         }

//         // If after retries the email still hasn't been sent, return an error
//         if (!success)
//         {
//             return StatusCode(500, new { Message = "Failed to send email after maximum retries." });
//         }

//         // If emails are successfully sent, return a success message
//         return Ok(new { Message = sendToAllUsers ? "Emails sent to all users." : "Emails sent to specified users." });
//     }
//     catch (Exception ex)
//     {
//         // Return a general error message if an unexpected error occurs
//         return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
//     }
// }


// ეს მუშაობს მხოლოდ ჩაწერაზე მეილის და ისე არ მუშაობს
// [HttpPost("send-email")]
// public IActionResult SendCustomEmail([FromBody] EmailRequestDto emailRequest)
// {
//     // Validate that subject and body are not empty
//     if (string.IsNullOrEmpty(emailRequest.Subject) || string.IsNullOrEmpty(emailRequest.Body))
//     {
//         return BadRequest(new { Message = "Subject and Body are required." });
//     }

//     // Check if the emails list is null or empty. If so, send to all users.
//     var sendToAllUsers = emailRequest.Emails == null || emailRequest.Emails.Count == 0;

//     // Fire-and-forget logic
//     _ = Task.Run(async () =>
//     {
//         try
//         {
//             await _emailService.SendEmailsAsync(
//                 sendToAllUsers ? new List<string>() : emailRequest.Emails,
//                 emailRequest.Subject,
//                 emailRequest.Body
//             );
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError($"Error occurred while sending emails: {ex.Message}");
//         }
//     });

//     // Return a response immediately
//     return Ok(new { Message = "Emails are being sent." });
// }

    // [HttpPost("send-email")]
    // public IActionResult SendCustomEmail([FromBody] EmailRequestDto emailRequest)
    // {
    //     // Validate input
    //     if (string.IsNullOrWhiteSpace(emailRequest.Subject) || string.IsNullOrWhiteSpace(emailRequest.Body))
    //     {
    //         return BadRequest(new { Message = "Subject and Body are required." });
    //     }

    //     // Fire-and-forget task to send emails asynchronously
    //     _ = Task.Run(async () =>
    //     {
    //         try
    //         {
    //             var emailList = await dbContext.MyUser
    //                 .Select(u => u.Email)
    //                 .ToListAsync();
    //             if (!emailList.Any())
    //             {
    //                 _logger.LogWarning("No valid email addresses found in the database.");
    //                 return;
    //             }

    //             // Send emails in batches
    //             const int batchSize = 100;
    //             await _emailService.SendEmailsAsync(emailList, emailRequest.Subject, emailRequest.Body, batchSize);
    //         }
    //         catch (Exception ex)
    //         {
    //             // Log any unexpected errors
    //             _logger.LogError($"Error occurred while sending emails: {ex.Message}");
    //         }
    //     });

    //     // Immediately return response
    //     return Ok(new { Message = "Emails are being sent to all users." });
    // }
[HttpPost("send-email")]
public async Task<IActionResult> SendCustomEmail([FromBody] EmailRequestDto emailRequest)
{
    // Validate that subject and body are provided
    if (string.IsNullOrEmpty(emailRequest.Subject) || string.IsNullOrEmpty(emailRequest.Body))
    {
        return BadRequest(new { Message = "Subject and Body are required." });
    }

    try
    {
        // Fetch emails: Use provided emails or fetch all users' emails from the database
        List<string> emailsToSend = emailRequest.Emails != null && emailRequest.Emails.Any()
            ? emailRequest.Emails
            : dbContext.MyUser
                .Select(user => user.Email)
                .ToList();

        // Ensure there are emails to send
        if (!emailsToSend.Any())
        {
            return BadRequest(new { Message = "No emails found to send." });
        }

        // Log the number of emails being sent
        _logger.LogInformation($"Starting to send emails to {emailsToSend.Count} recipients.");

        // Send emails in batches for better control and logging
        const int batchSize = 50; // Adjust batch size as needed
        int successCount = 0;

        for (int i = 0; i < emailsToSend.Count; i += batchSize)
        {
            // Get the current batch
            var emailBatch = emailsToSend.Skip(i).Take(batchSize).ToList();

            foreach (var email in emailBatch)
            {
                try
                {
_logger.LogInformation($"[{DateTime.UtcNow}] Sending email to {email}");
                await _emailService.SendEmailAsync(email, emailRequest.Subject, emailRequest.Body);
                _logger.LogInformation($"[{DateTime.UtcNow}] Email successfully sent to {email}");
                    successCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to send email to {email}: {ex.Message}");
                }
            }
        }

        _logger.LogInformation($"Finished sending emails. Total successfully sent: {successCount}");

        return Ok(new { Message = $"{successCount}/{emailsToSend.Count} emails sent successfully." });
    }
    catch (Exception ex)
    {
        _logger.LogError($"An error occurred while sending emails: {ex.Message}");
        return StatusCode(500, new { Message = "An error occurred while sending emails." });
    }
}






public class EmailRequestDto
{
    public string Subject { get; set; }
    public string Body { get; set; }
    public List<string> Emails { get; set; } // Optional: specify email addresses or leave empty for all users
}


        [HttpGet("{id}")]
        public IActionResult GetQuizById(int id)
        {
            var quiz = dbContext.MyQuiz
                .Include(q => q.Questions)
                    .ThenInclude(qa => qa.IncorrectAnswers)
                .Include(q => q.BonusQuestion)
                    .ThenInclude(bq => bq.CorrectAnswers)
                .Include(q => q.BonusQuestion)
                    .ThenInclude(bq => bq.IncorrectAnswers)
                .FirstOrDefault(q => q.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }

            return Ok(quiz);
        }
      [HttpPost]
        public IActionResult PostQuiz([FromBody] QuizDto quizDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Map QuizDto to Quiz entity
            var quizEntity = new Quiz
            {
                Time = quizDto.Time,
                Questions = quizDto.Questions?.Select(q => new Quiz.Question
                {
                    question = q.question,
                    correctanswer = q.correctanswer,
                    img = q.img,
                    IncorrectAnswers = q.IncorrectAnswers?.Select(ia => new Quiz.IncorrectAnswer
                    {
                        InccorectAnswer = ia.Answer
                    }).ToList()
                }).ToList(),

                BonusQuestion = quizDto.BonusQuestion != null ? new Quiz.BonusQuestionDetails
                {
                    question = quizDto.BonusQuestion.question,
                    img = quizDto.BonusQuestion.img,
                    CorrectAnswers = quizDto.BonusQuestion.CorrectAnswers?.Select(ca => new Quiz.correctanswers
                    {
                        correctanswer = ca.correctanswer
                    }).ToList(),
                    IncorrectAnswers = quizDto.BonusQuestion.IncorrectAnswers?.Select(ia => new Quiz.IncorrectAnswer
                    {
                        InccorectAnswer = ia.Answer
                    }).ToList(),
                    Coins = quizDto.BonusQuestion.Coins
                } : null
            };

            dbContext.MyQuiz.Add(quizEntity);
            dbContext.SaveChanges();

            return CreatedAtAction(nameof(GetQuizById), new { id = quizEntity.Id }, quizEntity);
        }




      [HttpDelete("{id}")]
        public IActionResult DeleteQuiz(int id)
        {
            var quiz = dbContext.MyQuiz
                .Include(q => q.Questions)
                .ThenInclude(qa => qa.IncorrectAnswers)
                .Include(q => q.BonusQuestion)
                .ThenInclude(bq => bq.IncorrectAnswers)
                .FirstOrDefault(q => q.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }

            dbContext.MyQuiz.Remove(quiz);
            dbContext.SaveChanges();

            return NoContent();
        }
        [HttpGet("time/{time}")]
        public IActionResult GetQuizByTime(string time)
        {
            try
            {
                time = Uri.UnescapeDataString(time);
                Console.WriteLine("Received time: " + time);

                var quizzes = dbContext.MyQuiz
                    .Include(q => q.Questions)
                        .ThenInclude(qa => qa.IncorrectAnswers)
                    // .Include(q => q.BonusQuestion)
                    //     .ThenInclude(bq => bq.CorrectAnswers)
                    // .Include(q => q.BonusQuestion)
                    //     .ThenInclude(bq => bq.IncorrectAnswers)
                    .Where(q => q.Time == time)
                    .ToList();

                if (!quizzes.Any())
                {
                    return NotFound(new { Message = "No quizzes found for the specified time." });
                }

                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving the quiz.", Error = ex.Message });
            }
        }
    }
}
