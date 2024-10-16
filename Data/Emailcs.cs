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
//         private const string SmtpPass = "xsmtpsib-6eb4b545578f7c5a27aaf563a92cfd781fbfbab62ada56f13bee07fc10c33726-NOGd32qLRaCnPhxp"; // Your SMTP Master Password
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
        private readonly string SmtpUser;
        private readonly string SmtpPass;

        public Emailcs(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            SmtpUser = Environment.GetEnvironmentVariable("7df9dc001@smtp-brevo.com"); // Store this in environment variables
            SmtpPass = Environment.GetEnvironmentVariable("xsmtpsib-6eb4b545578f7c5a27aaf563a92cfd781fbfbab62ada56f13bee07fc10c33726-NOGd32qLRaCnPhxp"); // Store this in environment variables
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
                    smtpClient.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(SmtpUser), // Use the same email as SMTP User
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email to {email}: {ex.Message}");
            }
        }
    }
}
