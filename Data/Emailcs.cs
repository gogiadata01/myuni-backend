// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Net;
// using System.Net.Mail;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;

// namespace MyUni.Data
// {
//     public class Emailcs
//     {
//         private readonly ApplicationDbContext dbContext;

//         public Emailcs(ApplicationDbContext dbContext)
//         {
//             this.dbContext = dbContext;
//         }

//         // Send emails in batches with retry logic
// public async Task SendEmailsAsync(List<string> emails, string subject, string body, int batchSize = 50)
// {
//     for (int i = 0; i < emails.Count; i += batchSize)
//     {
//         var batch = emails.Skip(i).Take(batchSize).ToList();

//         foreach (var email in batch)
//         {
//             // Send each email with retry logic
//             await SendEmailWithRetryAsync(email, subject, body);
//         }

//         // Add a delay between batches to avoid breaching Brevo's rate limits
//         await Task.Delay(5000); // 5-second delay between batches
//     }
// }

// private async Task SendEmailWithRetryAsync(string email, string subject, string body, int maxRetries = 3)
// {
//     int attempt = 0;
//     bool success = false;

//     while (attempt < maxRetries && !success)
//     {
//         try
//         {
//             using (var smtpClient = new SmtpClient("smtp-relay.brevo.com", 587)) // Brevo SMTP server
//             {
//                 smtpClient.Credentials = new NetworkCredential("80107d001@smtp-brevo.com", "wpLhKzNrxGvmgbUD"); // Brevo credentials
//                 smtpClient.EnableSsl = true; // Enable SSL for secure connection

//                 using (var mailMessage = new MailMessage())
//                 {
//                     mailMessage.From = new MailAddress("hello@myuni.ge"); // Replace with your sender email
//                     mailMessage.To.Add(email);
//                     mailMessage.Subject = subject;
//                     mailMessage.Body = body;
//                     mailMessage.IsBodyHtml = true;

//                     // Send email asynchronously
//                     await smtpClient.SendMailAsync(mailMessage);
//                     success = true; // Mark the email as successfully sent
//                     Console.WriteLine($"Email sent to {email} successfully.");
//                 }
//             }
//         }
//         catch (SmtpException ex)
//         {
//             attempt++;
//             Console.WriteLine($"Attempt {attempt} failed to send email to {email}: {ex.Message}");

//             // Retry on specific SMTP errors
//             if (ex.Message.Contains("rate limit"))
//             {
//                 // Handle rate limit error with a longer delay
//                 await Task.Delay(30000); // 30-second delay for rate limit issues
//             }
//             else
//             {
//                 // Handle other SMTP errors with a shorter delay
//                 await Task.Delay(5000); // 5-second delay for other errors
//             }
//         }
//     }

//     // If after retries the email still hasn't been sent, log an error
//     if (!success)
//     {
//         Console.WriteLine($"Failed to send email to {email} after {maxRetries} attempts.");
//     }
// }

//     }
// }

// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Net;
// using System.Net.Mail;
// using System.Threading.Tasks;

// namespace MyUni.Data
// {
//     public class Emailcs
//     {
//         private readonly ApplicationDbContext dbContext;

//         public Emailcs(ApplicationDbContext dbContext)
//         {
//             this.dbContext = dbContext;
//         }

//         // Send emails in batches with retry logic
//         public async Task SendEmailsAsync(List<string> emails, string subject, string body, int batchSize = 50)
//         {
//             for (int i = 0; i < emails.Count; i += batchSize)
//             {
//                 var batch = emails.Skip(i).Take(batchSize).ToList();

//                 foreach (var email in batch)
//                 {
//                     // Send each email with retry logic
//                     await SendEmailWithRetryAsync(email, subject, body);
//                 }

//                 // Add a delay between batches to avoid breaching Brevo's rate limits
//                 await Task.Delay(5000); // 5-second delay between batches
//             }
//         }

//         private async Task SendEmailWithRetryAsync(string email, string subject, string body, int maxRetries = 3)
//         {
//             int attempt = 0;
//             bool success = false;

//             while (attempt < maxRetries && !success)
//             {
//                 try
//                 {
//                     using (var smtpClient = new SmtpClient("smtp-relay.brevo.com", 587)) // Brevo SMTP server
//                     {
//                         smtpClient.Credentials = new NetworkCredential("80107d001@smtp-brevo.com", "wpLhKzNrxGvmgbUD"); // Brevo credentials
//                         smtpClient.EnableSsl = true; // Enable SSL for secure connection

//                         using (var mailMessage = new MailMessage())
//                         {
//                             mailMessage.From = new MailAddress("hello@myuni.ge"); // Replace with your sender email
//                             mailMessage.To.Add(email);
//                             mailMessage.Subject = subject;
//                             mailMessage.Body = body;
//                             mailMessage.IsBodyHtml = true;

//                             // Send email asynchronously
//                             await smtpClient.SendMailAsync(mailMessage);
//                             success = true; // Mark the email as successfully sent
//                             Console.WriteLine($"Email sent to {email} successfully.");
//                         }
//                     }
//                 }
//                 catch (SmtpException ex)
//                 {
//                     attempt++;
//                     Console.WriteLine($"Attempt {attempt} failed to send email to {email}: {ex.Message}");

//                     // Retry on specific SMTP errors
//                     if (ex.Message.Contains("rate limit"))
//                     {
//                         // Handle rate limit error with a longer delay
//                         await Task.Delay(30000); // 30-second delay for rate limit issues
//                     }
//                     else
//                     {
//                         // Handle other SMTP errors with a shorter delay
//                         await Task.Delay(5000); // 5-second delay for other errors
//                     }
//                 }
//             }

//             // If after retries the email still hasn't been sent, log an error
//             if (!success)
//             {
//                 Console.WriteLine($"Failed to send email to {email} after {maxRetries} attempts.");
//             }
//         }
//     }
// }




// ეს არის კოდი რომელიც პატარა bratch ებად უშვებს
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Net;
// using System.Net.Mail;
// using System.Threading.Tasks;

// namespace MyUni.Data
// {
//     public class Emailcs
//     {
//         private readonly ApplicationDbContext dbContext;

//         public Emailcs(ApplicationDbContext dbContext)
//         {
//             this.dbContext = dbContext;
//         }

// public async Task SendEmailsAsync(List<string> emails, string subject, string body, int batchSize = 100)
// {
//     // Split emails into smaller batches if needed
//     var batches = emails
//         .Select((email, index) => new { email, index })
//         .GroupBy(x => x.index / batchSize)  // Group emails into batches
//         .Select(g => g.Select(x => x.email).ToList())
//         .ToList();

//     // Send each batch asynchronously in parallel
//     var tasks = batches.Select(batch => SendEmailBatchAsync(batch, subject, body));
//     await Task.WhenAll(tasks);  // Wait for all tasks to complete
// }

// private async Task SendEmailBatchAsync(List<string> batch, string subject, string body)
// {
//     // Send emails in parallel for the current batch
//     var tasks = batch.Select(email => SendEmailWithRetryAsync(email, subject, body));
//     await Task.WhenAll(tasks);  // Wait for all emails in the batch to be sent
// }

// private async Task SendEmailWithRetryAsync(string email, string subject, string body, int maxRetries = 3)
// {
//     int attempt = 0;
//     bool success = false;

//     while (attempt < maxRetries && !success)
//     {
//         try
//         {
//             using (var smtpClient = new SmtpClient("smtp-relay.brevo.com", 587)) // Brevo SMTP server
//             {
//                 smtpClient.Credentials = new NetworkCredential("80107d001@smtp-brevo.com", "wpLhKzNrxGvmgbUD");
//                 smtpClient.EnableSsl = true;

//                 using (var mailMessage = new MailMessage())
//                 {
//                     mailMessage.From = new MailAddress("hello@myuni.ge"); // Sender email
//                     mailMessage.To.Add(email);
//                     mailMessage.Subject = subject;
//                     mailMessage.Body = body;
//                     mailMessage.IsBodyHtml = true;

//                     await smtpClient.SendMailAsync(mailMessage);
//                     success = true;  // If successful, mark the email as sent
//                     Console.WriteLine($"Email sent to {email}.");
//                 }
//             }
//         }
//         catch (SmtpException ex)
//         {
//             attempt++;
//             Console.WriteLine($"Attempt {attempt} failed to send email to {email}: {ex.Message}");

//             // Retry logic based on SMTP errors
//             if (ex.Message.Contains("rate limit"))
//             {
//                 await Task.Delay(30000); // Longer delay for rate limit errors
//             }
//             else
//             {
//                 await Task.Delay(5000); // Shorter delay for other errors
//             }
//         }
//     }

//     if (!success)
//     {
//         Console.WriteLine($"Failed to send email to {email} after {maxRetries} attempts.");
//     }
// }

//     }
// }





// ეს არის კოდი რომელიც დიდ ბრენჩად უშევებს
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MyUni.Data
{
    public class Emailcs
    {
        private readonly ApplicationDbContext dbContext;

        public Emailcs(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // Method to send emails in batches of 1000
        public async Task SendEmailsAsync(List<string> emails, string subject, string body, int batchSize = 1000)
        {
            // Split emails into smaller batches of 1000 emails
            var batches = emails
                .Select((email, index) => new { email, index })
                .GroupBy(x => x.index / batchSize)  // Group emails into batches
                .Select(g => g.Select(x => x.email).ToList())
                .ToList();

            // Send each batch asynchronously in parallel
            var tasks = batches.Select(batch => SendEmailBatchAsync(batch, subject, body));
            await Task.WhenAll(tasks);  // Wait for all tasks to complete
        }

        // Method to send a batch of emails
        private async Task SendEmailBatchAsync(List<string> batch, string subject, string body)
        {
            // Send emails in parallel for the current batch
            var tasks = batch.Select(email => SendEmailWithRetryAsync(email, subject, body));
            await Task.WhenAll(tasks);  // Wait for all emails in the batch to be sent
        }

        // Method to send an email with retry logic
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
                        smtpClient.Credentials = new NetworkCredential("80107d001@smtp-brevo.com", "wpLhKzNrxGvmgbUD");
                        smtpClient.EnableSsl = true;

                        using (var mailMessage = new MailMessage())
                        {
                            mailMessage.From = new MailAddress("hello@myuni.ge"); // Sender email
                            mailMessage.To.Add(email);
                            mailMessage.Subject = subject;
                            mailMessage.Body = body;
                            mailMessage.IsBodyHtml = true;

                            await smtpClient.SendMailAsync(mailMessage);
                            success = true;  // If successful, mark the email as sent
                            Console.WriteLine($"Email sent to {email}.");
                        }
                    }
                }
                catch (SmtpException ex)
                {
                    attempt++;
                    Console.WriteLine($"Attempt {attempt} failed to send email to {email}: {ex.Message}");

                    // Retry logic based on SMTP errors
                    if (ex.Message.Contains("rate limit"))
                    {
                        await Task.Delay(30000); // Longer delay for rate limit errors
                    }
                    else
                    {
                        await Task.Delay(5000); // Shorter delay for other errors
                    }
                }
            }

            if (!success)
            {
                Console.WriteLine($"Failed to send email to {email} after {maxRetries} attempts.");
            }
        }
    }
}
