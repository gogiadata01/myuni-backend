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
            foreach (var user in allUsers)
            {
                SendEmail(user.Email, subject, body);
            }
        }

private void SendEmail(string email, string subject, string body)
{
    using (var client = new SmtpClient("smtp.gmail.com", 465)) // Use Gmail's SMTP server
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



    }
}