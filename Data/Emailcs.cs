using System.Net.Mail;
using System.Net;

namespace MyUni.Data
{
    public class Emailcs
    {
        private readonly ApplicationDbContext dbContext;

        public Emailcs(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void SendEmailToAllUsers(string subject, string body)
        {
            // Get all users from the database
            var allUsers = dbContext.MyUser.ToList();

            // Loop through all users and send them an email
            // foreach (var user in allUsers)
            // {
            //     SendEmail(user.Email, subject, body);
            // }
            SendEmail("datagoia@gmail.com", "ylistavi", "momwove");
        }

private void SendEmail(string email, string subject, string body)
{
    try
    {
        using (var client = new SmtpClient("smtp.gmail.com", 587)) // Use Gmail's SMTP server
        {
            client.UseDefaultCredentials = false; 
            client.Credentials = new NetworkCredential("lukasordia@gmail.com", "kkki xudy ozln fesd"); // Use your App Password
            client.EnableSsl = true; // Enable SSL/TLS

            var mailMessage = new MailMessage
            {
                From = new MailAddress("lukasordia@gmail.com"), // Sender email
                Subject = subject,
                Body = body,
                IsBodyHtml = false, // Set to true if the body contains HTML
            };

            mailMessage.To.Add(email); // Add recipient's email
            client.Send(mailMessage);  // Send the email
        }
    }
  
    catch (SmtpException smtpEx)
    {
        // Log SMTP exception details
        Console.WriteLine($"SMTP Error: {smtpEx.Message}");
        throw; // Rethrow the exception if necessary
    }
    catch (Exception ex)
    {
        // Log general exception details
        Console.WriteLine($"Error: {ex.Message}");
        throw; // Rethrow the exception if necessary
    }
}



    }
}