using System.Web;
using Domain.Entities;
using Domain.Entities.Configurations;
using Microsoft.Extensions.Options;
using ZonefyDotnet.Helpers;
using ZonefyDotnet.Services.Interfaces;

namespace ZonefyDotnet.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IProducerService _producerService;
        private readonly EmailVerificationUrls _emailVerificationUrls;
        private readonly RabbitMQMessageBroker _rabbitMQMessageBroker;

        public NotificationService(
            IOptions<EmailVerificationUrls> emailVerificationUrls,
            IProducerService producerService,
            IOptions<RabbitMQMessageBroker> rabbitMQMessageBroker)
        {
            _producerService = producerService;
            _emailVerificationUrls = emailVerificationUrls.Value;
            _rabbitMQMessageBroker = rabbitMQMessageBroker.Value;
        }

        public void SendPasswordResetEmail(string receiverEmail, string name, string token)
        {
            EmailRequest request = new EmailRequest();
            request.ReceiverEmail = receiverEmail;
            request.EmailSubject = "Forget Password";
            request.TemplateName = "resetPassword";

            string link = _emailVerificationUrls.Reset;
            token = HttpUtility.UrlEncode(token);
            link = link.Replace("[Token]", token).Replace("[Email]", receiverEmail);
            request.Variables = new Dictionary<string, string>(){
                {"name", name},
                {"link", link}
            };
            var message = new Notification<EmailRequest>()
            {
                Type = "Email",
                Data = request
            };
            _producerService.SendMessage(message, _rabbitMQMessageBroker.QueueNotification);
        }

        public void SendPinAddEmail(string email, string firstName)
        {
            // throw new System.NotImplementedException();
        }

        public void SendPinUpdateEmail(string email, string firstName)
        {
            // throw new System.NotImplementedException();s
        }

        public void SendStaffAccountEmail(string email, string firstName) { }

        public void SendVerificationEmail(string receiverEmail, string name, string token)
        {
            EmailRequest request = new EmailRequest();
            request.ReceiverEmail = receiverEmail;
            request.EmailSubject = "Email verification";
            request.TemplateName = "verifyEmail";

            string link = _emailVerificationUrls.Verify;
            token = HttpUtility.UrlEncode(token);
            link = link.Replace("[Email]", request.ReceiverEmail).Replace("[Token]", token);
            request.Variables = new Dictionary<string, string>(){
                {"name", name},
                {"link", link}
            };
            var message = new Notification<EmailRequest>()
            {
                Type = "Email",
                Data = request
            };
            _producerService.SendMessage(message, _rabbitMQMessageBroker.QueueNotification);
        }

        public void Subscribe(string receiverEmail, string firstName, string lastName)
        {
            Subscriber subscriber = new Subscriber()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = receiverEmail
            };
            var message = new Notification<Subscriber>()
            {
                Type = "Subscribe",
                Data = subscriber
            };
            _producerService.SendMessage(message, _rabbitMQMessageBroker.QueueNotification);
        }
    }
}