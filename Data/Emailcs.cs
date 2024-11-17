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

private async Task SendEmailsInBatchesAsync(List<string> emails, string subject, string body, int batchSize = 50)
{
    for (int i = 0; i < emails.Count; i += batchSize)
    {
        var batch = emails.Skip(i).Take(batchSize).ToList();

        foreach (var email in batch)
        {
            await SendEmailWithRetryAsync(email, subject, body);
        }

        // Add a delay between batches to avoid breaching Brevo's rate limits
        await Task.Delay(5000); // 5-second delay
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
            using (var smtpClient = new SmtpClient("smtp-relay.brevo.com", 587))
            {
                smtpClient.Credentials = new NetworkCredential("80107d001@smtp-brevo.com", "wpLhKzNrxGvmgbUD");
                smtpClient.EnableSsl = true;

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("your-email@example.com");
                    mailMessage.To.Add(email);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    await smtpClient.SendMailAsync(mailMessage);
                    success = true;
                }
            }

            Console.WriteLine($"Email sent to {email}");
        }
        catch (SmtpException ex)
        {
            attempt++;
            Console.WriteLine($"Attempt {attempt} failed: {ex.Message}");
            
            // Handle rate limit errors with a longer delay
            if (ex.Message.Contains("rate limit"))
            {
                await Task.Delay(30000); // 30-second delay for rate limit issues
            }
            else
            {
                await Task.Delay(5000); // 5-second delay for other errors
            }
        }
    }
}


    }
}
