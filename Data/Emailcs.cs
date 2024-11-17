// using System.Net;
// using System.Net.Mail;
// using System.Threading.Tasks;

// namespace MyUni.Data
// {
//     public class Emailcs
//     {
//         private readonly ApplicationDbContext dbContext;
//         private const string SmtpServer = "smtp-relay.brevo.com";
//         private const int SmtpPort = 587;
//         private const string SmtpUser = "7df9dc001@smtp-brevo.com";
//         private const string SmtpPass = "xMyRkZOdmP7s6VKa"; 
//         public Emailcs(ApplicationDbContext dbContext)
//         {
//             this.dbContext = dbContext;
//         }

//         public async Task SendEmailToAllUsers(string subject, string body)
//         {
//             var users =  dbContext.MyUser.ToList(); // Assuming you have a Users DbSet

//             foreach (var user in users)
//             {
//                 await SendEmail(user.Email, subject, body);
//             }
//         }

//         private async Task SendEmail(string email, string subject, string body)
//         {
//             try
//             {
//                 using (var smtpClient = new SmtpClient(SmtpServer, SmtpPort))
//                 {
//                     smtpClient.Credentials = new NetworkCredential(SmtpUser, SmtpPass);
//                     smtpClient.EnableSsl = true;

//                     var mailMessage = new MailMessage
//                     {
//                         From = new MailAddress(SmtpUser), // Use the same email as SMTP User
//                         Subject = subject,
//                         Body = body,
//                         IsBodyHtml = true
//                     };

//                     mailMessage.To.Add(email);

//                     await smtpClient.SendMailAsync(mailMessage);
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Failed to send email to {email}: {ex.Message}");
//             }
//         }
//     }
// }

// using System.Net;
// using System.Net.Mail;
// using System.Threading.Tasks;

// namespace MyUni.Data
// {
//     public class Emailcs
//     {
//         private readonly ApplicationDbContext dbContext;
//         private const string SmtpServer = "smtp-relay.brevo.com";
//         private const int SmtpPort = 587;
//         private const string SmtpUser = "7df9dc001@smtp-brevo.com"; // Your Brevo SMTP login
//         private const string SmtpPass = "xMyRkZOdmP7s6VKa";         // Your Brevo SMTP password
//         private const string SenderEmail = "hello@myuni.ge";   // The verified sender email address

//         public Emailcs(ApplicationDbContext dbContext)
//         {
//             this.dbContext = dbContext;
//         }

//         public async Task SendEmailToAllUsers(string subject, string body)
//         {
//             var users = dbContext.MyUser.ToList(); // Assuming you have a Users DbSet

//             foreach (var user in users)
//             {
//                 await SendEmail(user.Email, subject, body);
//             }
//         }

//         private async Task SendEmail(string email, string subject, string body)
//         {
//             try
//             {
//                 using (var smtpClient = new SmtpClient(SmtpServer, SmtpPort))
//                 {
//                     smtpClient.Credentials = new NetworkCredential(SmtpUser, SmtpPass);
//                     smtpClient.EnableSsl = true; // Use SSL/TLS

//                     var mailMessage = new MailMessage
//                     {
//                         From = new MailAddress(SenderEmail), // Must use the verified sender email
//                         Subject = subject,
//                         Body = body,
//                         IsBodyHtml = true
//                     };

//                     mailMessage.To.Add(email);

//                     await smtpClient.SendMailAsync(mailMessage);
//                 }
//             }
//             catch (SmtpException smtpEx)
//             {
//                 Console.WriteLine($"SMTP Error: {smtpEx.Message}");
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Failed to send email to {email}: {ex.Message}");
//             }
//         }
//     }
// }
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyUni.Data
{
    public class Emailcs
    {
        private readonly ApplicationDbContext dbContext;
        private const string SmtpServer = "smtp-relay.brevo.com";
        private const int SmtpPort = 587;
        private const string SmtpUser = "7df9dc001@smtp-brevo.com"; // Your Brevo SMTP login
        private const string SmtpPass = "xMyRkZOdmP7s6VKa";         // Your Brevo SMTP password
        private const string SenderEmail = "hello@myuni.ge";   // The verified sender email address

        public Emailcs(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // Send emails in batches with retry logic
public async Task SendEmailsAsync(List<string> emails, string subject, string body, int batchSize = 50)
{
    for (int i = 0; i < emails.Count; i += batchSize)
    {
        var batch = emails.Skip(i).Take(batchSize).ToList();

        foreach (var email in batch)
        {
            // Send each email with retry logic
            await SendEmailWithRetryAsync(email, subject, body);
        }

        // Add a delay between batches to avoid breaching Brevo's rate limits
        await Task.Delay(5000); // 5-second delay between batches
    }
}

private async Task SendEmailWithRetryAsync(string email, string subject, string body, int maxRetries = 3)
{
    int attempt = 0;
    bool success = false;

    while (attempt < maxRetries && !success)
    {
        try
        {
            using (var smtpClient = new SmtpClient("smtp-relay.brevo.com", 587)) // Brevo SMTP server
            {
                smtpClient.Credentials = new NetworkCredential("80107d001@smtp-brevo.com", "wpLhKzNrxGvmgbUD"); // Brevo credentials
                smtpClient.EnableSsl = true; // Enable SSL for secure connection

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("hello@myuni.ge"); // Replace with your sender email
                    mailMessage.To.Add(email);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    // Send email asynchronously
                    await smtpClient.SendMailAsync(mailMessage);
                    success = true; // Mark the email as successfully sent
                    Console.WriteLine($"Email sent to {email} successfully.");
                }
            }
        }
        catch (SmtpException ex)
        {
            attempt++;
            Console.WriteLine($"Attempt {attempt} failed to send email to {email}: {ex.Message}");

            // Retry on specific SMTP errors
            if (ex.Message.Contains("rate limit"))
            {
                // Handle rate limit error with a longer delay
                await Task.Delay(30000); // 30-second delay for rate limit issues
            }
            else
            {
                // Handle other SMTP errors with a shorter delay
                await Task.Delay(5000); // 5-second delay for other errors
            }
        }
    }

    // If after retries the email still hasn't been sent, log an error
    if (!success)
    {
        Console.WriteLine($"Failed to send email to {email} after {maxRetries} attempts.");
    }
}

    }
}
