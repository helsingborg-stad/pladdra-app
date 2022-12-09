using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Mail;
using System.Net;
using System.Net.Security;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace UntoldGarden
{
    public class EmailHandler : MonoBehaviour
    {
        public string fromAdress;
        public TextAsset password;
        public string toAdress;
        public void SendEmail(string subject, string body)
        {
            StartCoroutine(SendEmailCoroutine(subject, body));
        }

        IEnumerator SendEmailCoroutine(string subject, string body)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            SmtpServer.Timeout = 10000;
            SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Port = 587;

            mail.From = new MailAddress(fromAdress);
            mail.To.Add(toAdress);

            mail.Subject = subject;
            mail.Body = body;

            SmtpServer.Credentials = new System.Net.NetworkCredential(fromAdress, password.text);
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };

            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
        }
    }
}