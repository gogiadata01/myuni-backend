﻿using Microsoft.AspNetCore.Http;
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

        public QuizController(ApplicationDbContext dbContext, Emailcs emailService)
        {
            this.dbContext = dbContext;
            _emailService = emailService;
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
        [HttpGet("reminder")]
public async Task<IActionResult> SendReminderForQuiz()
{
    try
    {
        var quizzes = await dbContext.MyQuiz
            .Include(card => card.Questions)
            .ThenInclude(incorrectAnswer => incorrectAnswer.IncorrectAnswers)
            .ToListAsync();

        var quiz = quizzes.FirstOrDefault();

        if (quiz == null)
        {
            return NotFound(new { Message = "Quiz not found." });
        }

        Console.WriteLine($"Attempting to parse time: {quiz.Time}");

        // Get the current year and construct the time string
        var currentYear = DateTime.Now.Year;
        var timeWithYear = $"{quiz.Time}/{currentYear}";

        // Parse the quiz time
        if (!DateTime.TryParseExact(timeWithYear, 
                                     "MM/dd HH:mm/yyyy", 
                                     CultureInfo.InvariantCulture, 
                                     DateTimeStyles.None, 
                                     out DateTime quizTime))
        {
            return BadRequest(new { Message = "Invalid quiz time format." });
        }

        // Get the local time zone for Georgia
        TimeZoneInfo localZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tbilisi");
        DateTime quizTimeInLocalZone = TimeZoneInfo.ConvertTime(quizTime, localZone);

        // Calculate the reminder time
        var reminderTime = quizTimeInLocalZone.AddMinutes(-30);
        var currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.Utc, localZone); // Get current local time

        // Log times for debugging
        Console.WriteLine($"Current Time (Local): {currentTime}");
        Console.WriteLine($"Quiz Time (Local): {quizTimeInLocalZone}");
        Console.WriteLine($"Reminder Time (Local): {reminderTime}");

        // Check if it's time to send the reminder
        if (currentTime >= reminderTime && currentTime <= quizTimeInLocalZone)
        {
            await _emailService.SendEmailToAllUsers(
                "Reminder: ქვიზი დაიწყება მალე სატესტო ემაილის გაგზავნის ცდა ",
                "ქვიზის დაწყებამდე დარჩენილი 30 წუთი."
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
        Console.WriteLine(ex.Message);
        return StatusCode(500, new { Message = "An error occurred while sending reminders." });
    }
}

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

    // Log incoming data for debugging
    Console.WriteLine("Received QuizDto:");
    Console.WriteLine($"Time: {quizDto.Time}");

    foreach (var question in quizDto.Questions)
    {
        Console.WriteLine($"Question: {question.question}");
        Console.WriteLine($"Correct Answer: {question.correctanswer}");
        foreach (var incorrectAnswer in question.IncorrectAnswers)
        {
            Console.WriteLine($"Incorrect Answer: {incorrectAnswer.Answer}");
        }
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
                InccorectAnswer = ia.Answer  // Mapping IncorrectAnswer to InccorectAnswer
            }).ToList()
        }).ToList(),
        
        // Map the BonusQuestion if present in the DTO
        BonusQuestion = quizDto.BonusQuestion != null ? new Quiz.BonusQuestionDetails
        {
            question = quizDto.BonusQuestion.question,
            correctanswer = quizDto.BonusQuestion.correctanswer,
            img = quizDto.BonusQuestion.img,
            IncorrectAnswers = quizDto.BonusQuestion.IncorrectAnswers?.Select(ia => new Quiz.IncorrectAnswer
            {
                InccorectAnswer = ia.Answer  // Map IncorrectAnswer
            }).ToList(),
            Coins = quizDto.BonusQuestion.Coins // Bonus coins for the question
        } : null
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
            .ThenInclude(q => q.IncorrectAnswers)
            .Include(q => q.BonusQuestion) // Make sure BonusQuestion is included
            .Where(quiz => quiz.Time == time)
            .ToList();

        if (!quizzes.Any())
        {
            return NotFound(new { Message = "No quizzes found for the specified time." });
        }

        // Ensure that the BonusQuestion's IncorrectAnswers is not null
        foreach (var quiz in quizzes)
        {
            if (quiz.BonusQuestion != null && quiz.BonusQuestion.IncorrectAnswers == null)
            {
                quiz.BonusQuestion.IncorrectAnswers = new List<IncorrectAnswer>(); // Initialize to empty list if null
            }
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
