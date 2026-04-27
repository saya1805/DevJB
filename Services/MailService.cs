using System.Net.Mail;
using MimeKit;
using static System.Net.Mime.MediaTypeNames;

namespace DevJBackend.Services
{
    public class MailService
    {
        public void sendOtpmail(string userEmail,string otp)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("devjourney0426@gmail.com"));
            email.To.Add(MailboxAddress.Parse(userEmail));
            email.Subject = "Dev Journey: Complete your verification";
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            { Text = $@"
                     <div style='font-family: sans-serif; line-height: 1.5;'>
                     <p>Hi there,</p>
                     <p>To verify your account, please use the following OTP:</p>
                     <h1 style='color: #4A90E2; letter-spacing: 5px;'>{otp}</h1>
                     <p style='font-size: 12px; color: #666;'>This code is valid for 10 minutes.</p>
                     <p>Happy Coding!<br><b>The Dev Journey Team</b></p>
                     </div>"
            };
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate("devjourney0426@gmail.com", "wxxo rzqk uwdz owbp");
            smtp.Send(email);
            smtp.Disconnect(true);
    }
    }
}
