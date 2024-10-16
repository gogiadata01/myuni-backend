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

using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MyUni.Data
{
    public class Emailcs
    {
        private readonly ApplicationDbContext dbContext;
        private const string SmtpServer = "smtp-relay.brevo.com";
        private const int SmtpPort = 587;
        private const string SmtpUser = "lukasordia@myuni.ge"; // Match with your Brevo verified sender
        private const string SmtpPass = "xMyRkZOdmP7s6VKa";  // Ensure this is correct

        public Emailcs(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task SendEmailToAllUsers(string subject, string body)
        {
            var users = dbContext.MyUser.ToList(); // Assuming you have a Users DbSet

            foreach (var user in users)
            {
                await SendEmail(user.Email, subject, body);
            }
        }

        private async Task SendEmail(string email, string subject, string body)
        {
            try
            {
                using (var smtpClient = new SmtpClient(SmtpServer, SmtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(SmtpUser, SmtpPass);
                    smtpClient.EnableSsl = true; // Use SSL/TLS

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(SmtpUser), // Must match the verified sender email
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP Error: {smtpEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email to {email}: {ex.Message}");
            }
        }
    }
}
