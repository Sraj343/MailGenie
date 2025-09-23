using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailGenie.Domain.Model.Mail
{
    class MailModel
    {
        
    }

    public class EmailTemplate
    {
        public int TemplateId { get; set; }
        public string TemplateContent { get; set; } = string.Empty;
        public string SubjectContent { get; set; }
        public string TemplateType { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; } = DateTime.UtcNow; 
        public bool IsActive { get; set; } = true; 
    }


    public class ApplicantDetailModel
    {
        public string Email { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
        public string ResumeLink { get; set; } = string.Empty;
        public string ApplierName { get; set; } = string.Empty;
        
    }

}
