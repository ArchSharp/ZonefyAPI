using Application.Helpers;
using Domain.Entities;

namespace ZonefyDotnet.Services.Interfaces
{
    public interface IEmailService : IAutoDependencyService
    {
        void SendEmailUsingMailKit(EmailRequest email);
        //Task<SuccessResponse<object>> SendEmailUsingSendGrid(Email email);
        string GetEmailTemplate<T>(string emailTemplate, T emailTemplateModel);
        string ImageCID(string imageName, string extension);
    }
}
