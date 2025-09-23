using MailGenie.Domain;
using MailGenie.Domain.Model.EmailTemplateModel;
using MailGenie.Domain.Model.Mail;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailGenie.Infra
{
    public class EmailTemplateService: IEmailTemplateService
    {
        private readonly AppDbContext _dbContext;
        public EmailTemplateService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddEmailTemplate(EmailTemplate emailTemplate)
        {
            try
            {

                var mailTemplate = new EmailTemplate
                {
                    TemplateContent = emailTemplate.TemplateContent,
                    TemplateType = "Interview",
                    SubjectContent = emailTemplate.SubjectContent,
                    Position = emailTemplate.Position,
                    CreateDate = DateTime.UtcNow,
                    IsActive = true
                };

                _dbContext.Mailtemplates.Add(mailTemplate);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<EmailTemplate>> GetTemplateContent()
        {
            var result = await _dbContext.Mailtemplates
                                         .AsNoTracking()
                                         .ToListAsync();

            return result;
        }
    }
}
