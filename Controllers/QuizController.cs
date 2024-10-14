using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyUni.Data;
using MyUni.Models;
using MyUni.Models.Entities;

namespace MyUni.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly Emailcs _emailService;  // Correctly declare the service

        public QuizController(ApplicationDbContext dbContext, Emailcs emailService)  // Inject both services
        {
            this.dbContext = dbContext;
            _emailService = emailService;  // Assign the email service
        }

        // GET: api/Quiz
        [HttpGet]
        public List<Quiz> GetAllQuizzes()
        {
            return dbContext.MyQuiz
                .Include(card => card.Questions)
                .ThenInclude(incorrectAnswer => incorrectAnswer.IncorrectAnswers)
                .ToList();
        }



        [HttpGet("reminder")]
        public IActionResult SendReminderForQuiz()
        {
            try
            {
                var quizzes = GetAllQuizzes(); 
                var quiz = quizzes.FirstOrDefault(); // Directly use the list here

                if (quiz == null)
                {
                    return NotFound(new { Message = "Quiz not found." });
                }

                Console.WriteLine("Received time for reminder: " + quiz.Time);
                
                // Get the current year
                var currentYear = DateTime.Now.Year;

                // Append the year to the quiz time string
                string quizTimeString = $"{currentYear}/{quiz.Time}"; // This will format to "YYYY/MM/DD HH:mm"
                
                // Define the expected format
                string expectedFormat = "yyyy/MM/dd HH:mm"; // Adjusted to the format used
                
                // Attempt to parse the time
                if (!DateTime.TryParseExact(quizTimeString, expectedFormat, 
                    CultureInfo.InvariantCulture, 
                    DateTimeStyles.None, out var quizTime))
                {
                    return BadRequest(new { Message = "Invalid quiz time format." });
                }

                // Calculate the reminder time (30 minutes before the quiz)
                var reminderTime = quizTime.AddMinutes(-30);
                var currentTime = DateTime.Now;

                if (currentTime >= reminderTime)
                {
                    _emailService.SendEmailToAllUsers(
                        "Reminder: ქვიზი დაიწყება მალე",
                        "ქვიზის დაწყებამდე დარჩენიალია 30 წუთი."
                    );

                    return Ok(new { Message = "Reminder emails have been sent to all users." });
                }
                else
                {
                    return BadRequest(new { Message = "It's too early to send a reminder. Try again closer to the quiz time." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while sending the reminder.", Error = ex.Message });
            }
        }



        // GET: api/Quiz/5
        [HttpGet("{id}")]
        public IActionResult GetQuizById(int id)
        {
            var quiz = dbContext.MyQuiz
                .Include(card => card.Questions)
                .ThenInclude(incorrectAnswer => incorrectAnswer.IncorrectAnswers)
                .FirstOrDefault(card => card.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }

            return Ok(quiz);
        }

        // POST: api/Quiz
        [HttpPost]
        public IActionResult PostQuiz([FromBody] QuizDto quizDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Log incoming data
            Console.WriteLine("Received QuizDto:");
            Console.WriteLine($"Time: {quizDto.Time}");
            foreach (var question in quizDto.Questions)
            {
                Console.WriteLine($"Question: {question.question}");
                Console.WriteLine($"Correct Answer: {question.correctanswer}");
                foreach (var incorrectAnswer in question.IncorrectAnswers)
                {
                    Console.WriteLine($"Incorrect Answer: {incorrectAnswer.InccorectAnswer}");
                }
            }

            var quizEntity = new Quiz
            {
                Time = quizDto.Time,
                Questions = quizDto.Questions?.Select(q => new Quiz.Question
                {
                    question = q.question,
                    correctanswer = q.correctanswer,
                    img = q.img,
                    IncorrectAnswers = q.IncorrectAnswers?.Select(ia => new Quiz.inccorectanswer
                    {
                        InccorectAnswer = ia.InccorectAnswer
                    }).ToList()
                }).ToList()
            };

            dbContext.MyQuiz.Add(quizEntity);
            dbContext.SaveChanges();

            return CreatedAtAction(nameof(GetQuizById), new { id = quizEntity.Id }, quizEntity);
        }

        // DELETE: api/Quiz/5
        [HttpDelete("{id}")]
        public IActionResult DeleteQuiz(int id)
        {
            var quiz = dbContext.MyQuiz
                .Include(q => q.Questions)
                    .ThenInclude(q => q.IncorrectAnswers)
                .FirstOrDefault(q => q.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }

            dbContext.MyQuiz.Remove(quiz);
            dbContext.SaveChanges();

            return NoContent();
        }

        // GET: api/Quiz/time/{time}
        [HttpGet("time/{time}")]
        public IActionResult GetQuizByTime(string time)
        {
            try
            {
                time = Uri.UnescapeDataString(time);
                Console.WriteLine("Received time: " + time);

                var quizzes = dbContext.MyQuiz
                    .Include(card => card.Questions)
                    .ThenInclude(incorrectAnswer => incorrectAnswer.IncorrectAnswers)
                    .Where(quiz => quiz.Time == time)
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
