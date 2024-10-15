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
        private readonly MailgunService _emailService;

public QuizController(ApplicationDbContext dbContext, MailgunService emailService) // Removed trailing comma
{
    this.dbContext = dbContext;
    _emailService = emailService; // Assign the injected email service
}


        // GET: api/Quiz
        [HttpGet]
        public IActionResult GetAllQuizzes()
        {
            var quizzes = dbContext.MyQuiz
                .Include(card => card.Questions)
                .ThenInclude(incorrectAnswer => incorrectAnswer.IncorrectAnswers)
                .ToList();

            return Ok(quizzes);
        }
        // In the SendReminderForQuiz method
public async Task<IActionResult> SendReminderForQuiz() // Make the method async
{
    // Other code...

    await _emailService.SendEmailAsync("ah", "test"); // Await the async call
    return Ok("asd");
}


        // GET: api/Quiz/reminder
        // [HttpGet("reminder")]
        // public IActionResult SendReminderForQuiz()
        // {
        //     // try
        //     // {
        //     //     var quizzes = dbContext.MyQuiz
        //     //         .Include(card => card.Questions)
        //     //         .ThenInclude(incorrectAnswer => incorrectAnswer.IncorrectAnswers)
        //     //         .ToList();

        //     //     var quiz = quizzes.FirstOrDefault();

        //     //     if (quiz == null)
        //     //     {
        //     //         return NotFound(new { Message = "Quiz not found." });
        //     //     }

        //     //     Console.WriteLine($"Attempting to parse time: {quiz.Time}");

        //     //     // Get the current year and construct the time string
        //     //     var currentYear = DateTime.Now.Year;
        //     //     var timeWithYear = $"{quiz.Time}/{currentYear}";

        //     //     // Parse the quiz time
        //     //     // if (!DateTime.TryParseExact(timeWithYear, 
        //     //     //                              "MM/dd HH:mm/yyyy", 
        //     //     //                              CultureInfo.InvariantCulture, 
        //     //     //                              DateTimeStyles.None, 
        //     //     //                              out DateTime quizTime))
        //     //     // {
        //     //     //     return BadRequest(new { Message = "Invalid quiz time format." });
        //     //     // }

        //     //     // // Get the local time zone for Georgia
        //     //     // TimeZoneInfo localZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tbilisi");
        //     //     // DateTime quizTimeInLocalZone = TimeZoneInfo.ConvertTime(quizTime, localZone);

        //     //     // // Calculate the reminder time
        //     //     // var reminderTime = quizTimeInLocalZone.AddMinutes(-30);
        //     //     // var currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.Utc, localZone); // Get current local time

        //     //     // // Log times for debugging
        //     //     // Console.WriteLine($"Current Time (Local): {currentTime}");
        //     //     // Console.WriteLine($"Quiz Time (Local): {quizTimeInLocalZone}");
        //     //     // Console.WriteLine($"Reminder Time (Local): {reminderTime}");

        //     //     // Check if it's time to send the reminder
        //     //     // if (currentTime >= reminderTime && currentTime <= quizTimeInLocalZone)
        //     //     // {
        //     //     //     _emailService.SendEmailToAllUsers(
        //     //     //         "Reminder: ქვიზი დაიწყება მალე",
        //     //     //         "ქვიზის დაწყებამდე დარჩენილი 30 წუთი."
        //     //     //     );

        //     //     //     return Ok(new { Message = "Reminder emails have been sent to all users." });
        //     //     // }
        //     //     // else
        //     //     // {
        //     //     //     return BadRequest(new { Message = "It's too early to send a reminder. Try again closer to the quiz time." });
        //     //     // }
        //     //     _emailService.SendEmailToAllUsers(
        //     //         "qe chamkari trakshi",
        //     //         "ah yeah"
        //     //     );

        //     //     return Ok(new {message = "chemi yle cheidevi chemi yle"});
        //     // }
        //     // catch (Exception ex)
        //     // {
        //     //     Console.WriteLine(ex.Message);
        //     // }

        //    _emailService.SendEmailAsync("ah","test");
        //    return Ok("asd");
        // }

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
