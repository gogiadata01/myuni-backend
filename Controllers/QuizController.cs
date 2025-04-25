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
// [HttpGet]
// public async Task<IActionResult> GetAllQuizzes()
// {
//     try
//     {
//         var quizzes = await dbContext.MyQuiz
//             .AsNoTracking()
//             .Select(q => new
//             {
//                 q.Id,
//                 q.Time,
//                 Questions = q.Questions.Select(question => new
//                 {
//                     question.question,
//                     question.correctanswer,
//                     IncorrectAnswers = question.IncorrectAnswers.Select(ia => ia.InccorectAnswer).ToList()
//                 }).ToList()
//             })
//             .ToListAsync();

//         return Ok(quizzes);
//     }
//     catch (Exception ex)
//     {
//         _logger.LogError(ex, "Error fetching quizzes");
//         return StatusCode(500, "Internal Server Error");
//     }
// }

[HttpGet]
public async Task<IActionResult> GetAllQuizzes()
{
    try
    {
        var quizzes = await dbContext.MyQuiz
            .AsNoTracking()
            .Include(q => q.Questions)
                .ThenInclude(q => q.IncorrectAnswers) // Ensure correct answers are fetched
            .Select(q => new
            {
                q.Id,
                q.Time,
                Questions = q.Questions.Select(question => new
                {
                    question.question,
                    question.correctanswer,
                    IncorrectAnswers = question.IncorrectAnswers
                        .Select(ia => new {ia.InccorectAnswer }) // Keeping structure as in old code
                        .ToList()
                }).ToList()
            })
            .ToListAsync();

        return Ok(quizzes);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error fetching quizzes");
        return StatusCode(500, "Internal Server Error");
    }
}


        [HttpPost("submit-quiz")]
        public async Task<IActionResult> SubmitQuiz([FromBody] QuizSubmissionDto submission)
        {
            var user = await dbContext.MyUser
                .Include(u => u.Quizes)
                .ThenInclude(q => q.QuizQuestions)
                .ThenInclude(q => q.BadAnswers)
                .FirstOrDefaultAsync(u => u.Id == submission.UserId);

            if (user == null)
                return NotFound("User not found.");

            var quizHistory = new User.QuizHistory
            {
                img = submission.Img,
                QuizQuestions = new List<User.questions>()
            };

            foreach (var q in submission.QuizQuestions)
            {
                var question = new User.questions
                {
                    question = q.Question,
                    correctanswer = q.CorrectAnswer,
                    UserAnswer = q.UserAnswer,
                    img = q.Img,
                    BadAnswers = q.BadAnswers?.Select(bad => new User.BadAnswer
                    {
                        badanswer = bad
                    }).ToList()
                };

                quizHistory.QuizQuestions.Add(question);
            }

            user.Quizes.Add(quizHistory);
            await dbContext.SaveChangesAsync();

            return Ok("Quiz submitted and saved successfully.");
        }


[HttpPost("archive")]
public IActionResult GetAndArchiveQuizzes()
{
    try
    {
        // Retrieve quizzes from the database
        var quizzes = dbContext.MyQuiz
            .Include(q => q.Questions)
                .ThenInclude(qa => qa.IncorrectAnswers)
            .Include(q => q.BonusQuestion)
                .ThenInclude(bq => bq.CorrectAnswers)
            .Include(q => q.BonusQuestion)
                .ThenInclude(bq => bq.IncorrectAnswers)
            .ToList();

        // Check if quizzes were retrieved
        if (quizzes == null || !quizzes.Any())
        {
            return BadRequest(new { message = "No quizzes found to archive." });
        }

        // Archive the quizzes
        var archivedQuizzes = quizzes.Select(quizEntity =>
        {
            // Skip null quiz entities
            if (quizEntity == null) return null;

            // Archive the quiz data
            var archivedQuiz = new QuizArchive
            {
                Time = quizEntity.Time ?? "Unknown Time",
                Questions = quizEntity.Questions?.Select(q => new ArchivedQuestion
                {
                    question = q.question ?? "No question provided",
                    correctanswer = q.correctanswer ?? "No correct answer",
                    img = q.img ?? "No image",
                    IncorrectAnswers = q.IncorrectAnswers?.Select(ia => new ArchivedIncorrectAnswer
                    {
                        InccorectAnswer = ia.InccorectAnswer ?? "No incorrect answer"
                    }).ToList() ?? new List<ArchivedIncorrectAnswer>()
                }).ToList() ?? new List<ArchivedQuestion>(),

                // Handle bonus question
                BonusQuestion = quizEntity.BonusQuestion != null ? new ArchivedBonusQuestionDetails
                {
                    question = quizEntity.BonusQuestion.question ?? "No bonus question",
                    img = quizEntity.BonusQuestion.img ?? "No bonus image",
                    Coins = quizEntity.BonusQuestion.Coins,
                    CorrectAnswers = quizEntity.BonusQuestion.CorrectAnswers?.Select(ca => new ArchivedCorrectAnswer
                    {
                        correctanswer = ca.correctanswer ?? "No correct answer"
                    }).ToList() ?? new List<ArchivedCorrectAnswer>(),

                    IncorrectAnswers = quizEntity.BonusQuestion.IncorrectAnswers?.Select(ia => new ArchivedIncorrectAnswer
                    {
                        InccorectAnswer = ia.InccorectAnswer ?? "No incorrect answer"
                    }).ToList() ?? new List<ArchivedIncorrectAnswer>()
                } : null // Handle null BonusQuestion gracefully
            };

            return archivedQuiz;
        }).Where(q => q != null).ToList(); // Filter out any null quizzes

        // If valid archives were found, add them to the archive database
        if (archivedQuizzes.Any())
        {
            dbContext.MyQuizArchive.AddRange(archivedQuizzes);
            dbContext.SaveChanges(); // Commit to the database
            return Ok(new { message = "Quizzes archived successfully." });
        }

        return BadRequest(new { message = "No valid quizzes to archive." });
    }
    catch (Exception ex)
    {
        // Handle any unexpected errors
        return StatusCode(500, new { message = "An error occurred while archiving quizzes.", error = ex.Message });
    }
}

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
        await _emailService.SendEmailsAsync(new List<string> { email }, emailRequest.Subject, emailRequest.Body);
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
