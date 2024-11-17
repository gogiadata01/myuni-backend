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

        // Method to send email to specified users or all users
        public async Task SendEmailsAsync(string subject, string body, List<string> specificEmails = null)
        {
            // If specific emails are provided, use them; otherwise, get all user emails from the database
            var emails = specificEmails?.Any() == true
                ? specificEmails
                : await dbContext.MyUser.Select(user => user.Email).ToListAsync();

            foreach (var email in emails)
            {
                await SendEmailAsync(email, subject, body);
            }
        }

private async Task SendEmailAsync(string email, string subject, string body)
{
    try
    {
        using (var smtpClient = new SmtpClient(SmtpServer, SmtpPort))
        {
            smtpClient.Credentials = new NetworkCredential(SmtpUser, SmtpPass);
            smtpClient.EnableSsl = true;

            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(SenderEmail);
                mailMessage.To.Add(email);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"Email sent successfully to {email}");
            }
        }
    }
    catch (SmtpException smtpEx)
    {
        Console.WriteLine($"SMTP Error for {email}: {smtpEx.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to send email to {email}: {ex.Message}");
    }
}

    }
}
