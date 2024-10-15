using Mailgun.Net;
using Mailgun.Net.Messages;
using Microsoft.Extensions.Configuration;

public class MailgunService
{
    private readonly MailgunClient _client;

    public MailgunService(IConfiguration configuration)
    {
        var apiKey = configuration["d010bdaf-b4855329"];
        var domain = configuration["myuni.ge"];
        _client = new MailgunClient(apiKey, domain);
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var message = new Message
        {
            From = "luka@myuni.ge", // Change to your from address
            To = to,
            Subject = subject,
            Text = body
        };

        await _client.Messages.SendAsync(message);
    }
}
