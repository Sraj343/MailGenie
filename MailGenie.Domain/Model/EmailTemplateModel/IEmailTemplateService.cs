using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailGenie.Domain.Model.EmailTemplateModel;
using MailGenie.Domain.Model.Mail;

namespace MailGenie.Domain.Model.EmailTemplateModel
{
    public interface IEmailTemplateService
    {
        Task<bool> AddEmailTemplate(EmailTemplate emailTemplate);
        Task<List<EmailTemplate>> GetTemplateContent();
    }
}
