using MailGenie.Domain.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailGenie.Domain.Model.Mail
{
    public interface IMailService
    {
        Task<MailResult> SendMail(string excelFilePath);
        Task<bool> AddEmailTemplate(EmailTemplate emailTemplate);
    }
}
