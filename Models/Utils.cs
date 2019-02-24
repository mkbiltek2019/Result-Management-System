using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace PRMS.Models
{
    public class Utils
    {
        public static void SendMail(string Name, string Email, string Subject, string Message)
        {
            string body = "<div style='border: medium solid White; width: 1000px; height: 700px;font-family: arial,sans-serif; font-size: 17px;'>";
            body += "<h3 style='background-color: blue; margin-top:0px;'>Admin Pstu</h3>";
            body += "<br />";
            body += "Dear " + Name + ",";
            body += "<br />";
            body += "<p>" + Message + "</p>";
            body += "Thanks";
            body += "<br />";
            body += "</div>";

            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(Email);
                mail.From = new MailAddress("resultpstu@gmail.com");
                mail.Subject = Subject;

                mail.Body = body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("resultpstu", "RoGaSeUqAhRiVnAt"); // Enter seders User name and password   
                smtp.EnableSsl = true;
                smtp.Send(mail);
                // WriteToFile("Email sent successfully to: " + name + " " + email);
            }
            catch (System.Net.Mail.SmtpException e)
            {

            }

        }
    }
}