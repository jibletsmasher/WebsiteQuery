using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.ComponentModel;

namespace WebsiteQuery
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://websitequery.azurewebsites.net")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {
        private string websiteURI = "https://www.google.com/";
        Timer timer;
        EmailInfo emailInfo;

        [WebMethod]
        public void WebsiteQuery(string password)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(websiteURI);

            EmailInfo emailInfo = new EmailInfo();
            emailInfo.password = password;
            emailInfo.request = request;

            timer = new Timer(QuerySite, emailInfo, 0, 60000);
        }

        private void QuerySite(Object emailInfo)
        {
            HttpWebRequest request = ((EmailInfo)emailInfo).request;
            string password = ((EmailInfo)emailInfo).password;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    break;
                case HttpStatusCode.OK:
                    SendEmail(password, "Get those camp sites!", "The status of " + websiteURI + " is OK.. HURRY!!");
                    break;
                default:
                    SendEmail(password, websiteURI + " status change", "The status of " + websiteURI + " has changed.");
                    break;
            }
        }

        private void SendEmail(string password, string subject, string messageBody)
        {
            System.Diagnostics.Debug.WriteLine("starting");
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("jibletsservices@gmail.com", password);
            client.Host = "smtp.gmail.com";

            // Specify the e-mail sender.
            // Create a mailing address that includes a UTF8 character
            // in the display name.
            MailAddress from = new MailAddress("jibletsservices@gmail.com",
               "Paul Bradley",
            System.Text.Encoding.UTF8);

            // Set destinations for the e-mail message.
            MailAddress to = new MailAddress("bradleypaul800@gmail.com");

            // Specify the message content.
            MailMessage message = new MailMessage(from, to);
            message.Body = messageBody;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Subject = subject;
            message.SubjectEncoding = System.Text.Encoding.UTF8;

            client.Send(message);

            // Clean up.
            message.Dispose();
        }
    }

    public class EmailInfo
    {
        public EmailInfo()
        {

        }

        public string password { get; set; }
        public HttpWebRequest request { get; set; }
    }
}
