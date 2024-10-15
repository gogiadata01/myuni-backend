using System.Net.Mail;
using System.Net;

namespace MyUni.Data
{
    public class Emailcs // 'public' modifier should be here
    {
        private readonly ApplicationDbContext dbContext;

        public Emailcs(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void SendEmailToAllUsers(string subject, string body)
        {
            // Your code here
        }

        private void SendEmail(string email, string subject, string body)
        {
            // Your code here
        }
    }
}
