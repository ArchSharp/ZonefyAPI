using Domain.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Utils;
using RazorEngineCore;
using System.Text;
using ZonefyDotnet.DTOs;
using ZonefyDotnet.Services.Interfaces;

namespace ZonefyDotnet.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly EmailSender _sender;
        //private readonly ISendGridClient _sendGridClient;
        //private readonly SendGridEmailSettings _sendGridEmailSettings;
        private BodyBuilder builder = new BodyBuilder();

        public EmailService(
            IOptions<EmailSender> sender
            //ISendGridClient sendGridClient,
            //IOptions<SendGridEmailSettings> sendGridEmailSettings
            )
        {
            _sender = sender.Value;
            //_sendGridClient = sendGridClient;
            //_sendGridEmailSettings = sendGridEmailSettings.Value;
        }

        public string GetEmailTemplate<T>(string emailTemplate, T emailTemplateModel)
        {
            string mailTemplate = LoadTemplate(emailTemplate);

            IRazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate modifiedMailTemplate = razorEngine.Compile(mailTemplate);

            return modifiedMailTemplate.Run(emailTemplateModel);
        }
        public void SendEmailUsingMailKit(EmailRequest payload)
        {
            var emailTemplate = LoadTemplate(payload.TemplateName);
            //string messageBody = string.Format(emailTemplate, payload.Variables["name"], payload.Variables["link"]);

            var emailObject = new MimeMessage();
            emailObject.From.Add(new MailboxAddress("ArchDemy", _sender.Email));
            emailObject.To.Add(new MailboxAddress(payload.Variables["name"], payload.ReceiverEmail));
            emailObject.Subject = payload.EmailSubject;
            //emailObject.Body = new TextPart(TextFormat.Html) { Text = messageBody };



            /*TextPart htmlPart = new TextPart(TextFormat.Html) { Text = messageBody };
            MultipartRelated multipartRelated = new MultipartRelated();
            multipartRelated.Add(htmlPart);
            EmbedImage(multipartRelated, ImagePath("mail","png"), "mail");

            emailObject.Body = multipartRelated;*/
            var mail = ImageCID("mail", "png");
            var twitter = ImageCID("twitter", "png");
            var linkedin = ImageCID("linkedin", "png");
            var facebook = ImageCID("facebook", "png");

            builder.HtmlBody = string.Format(emailTemplate, mail, payload.Variables["name"], payload.Variables["link"], twitter, linkedin, facebook);
            emailObject.Body = builder.ToMessageBody();

            //port 587 uses SecureSocketOptions.startTls
            //port 465 uses SecureSocketOptions.Auto
            using var smtp = new SmtpClient();
            smtp.Connect(_sender.Host, _sender.Port, SecureSocketOptions.Auto);
            //smtp.Connect(_sender.Host, _sender.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_sender.Email, _sender.Password);
            smtp.Send(emailObject);
            smtp.Disconnect(true);
        }

        private void EmbedImage(MultipartRelated multipartRelated, string imagePath, string contentId)
        {
            MimePart imagePart = new MimePart("image", "png")
            {
                ContentId = contentId,
                ContentDisposition = new ContentDisposition(ContentDisposition.Inline),
                ContentTransferEncoding = ContentEncoding.Base64
            };

            // Load the image file and set it as the content of the MimePart
            using (var stream = File.OpenRead(imagePath))
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                memoryStream.Position = 0;
                imagePart.Content = new MimeContent(memoryStream);
            }

            // Add the MimePart to the multipart/related container
            multipartRelated.Add(imagePart);
        }

        private string LoadTemplate(string emailTemplate)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string upThreeLevels = Path.Combine(baseDir, "..\\..\\..\\..\\");

            string templateDir = Path.Combine(upThreeLevels, "ZonefyDotnet/Files/MailTemplates");
            string templatePath = Path.Combine(templateDir, $"{emailTemplate}.html");

            using FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);

            string mailTemplate = streamReader.ReadToEnd();
            streamReader.Close();

            return mailTemplate;
        }

        public string ImageCID(string imageName, string extension)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string upThreeLevels = Path.Combine(baseDir, "..\\..\\..\\..\\");
            string templateDir = Path.Combine(upThreeLevels, "ZonefyDotnet\\Files\\Images");
            string filePath = Path.Combine(templateDir, $"{imageName}.{extension}");

            //Console.WriteLine("tPath: ",filePath);

            var image = builder.LinkedResources.Add(filePath);

            return image.ContentId = MimeUtils.GenerateMessageId();
        }
    }
}
