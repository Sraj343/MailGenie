using MailGenie.Domain;
using MailGenie.Domain.Model.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MailGenie.Domain.Response;

namespace MailGenie.Infra
{
    public class MailService : IMailService
    {
        private readonly AppDbContext _dbContext;
        private readonly SmtpSetting _smtpSetting;

        public MailService(AppDbContext dbContext, IOptions<SmtpSetting> smtpSetting)
        {
            _dbContext = dbContext;
            _smtpSetting = smtpSetting.Value;
        }


        public async Task<MailResult> SendMail(string excelFilePath)
        {
            var data = await ReadExcelData(excelFilePath);

            var result = await GenerateMail(data);

            return result;
        }

        public async Task<List<ApplicantDetailModel>> ReadExcelData(string excelFilePath)
        {
            List<ApplicantDetailModel> result = new List<ApplicantDetailModel>();
            try
            {
                using (var excelPackage = new ExcelPackage(new FileInfo(excelFilePath)))
                {
                    var worksheet = excelPackage.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowCount; row++)
                    {

                        var email = Convert.ToString(worksheet.Cells[row, 1].Value);
                        var companyName = Convert.ToString(worksheet.Cells[row, 2].Value);
                        var positionName = Convert.ToString(worksheet.Cells[row, 3].Value);
                        var resumeLink = Convert.ToString(worksheet.Cells[row, 4].Value);
                        var applierName = Convert.ToString(worksheet.Cells[row, 5].Value);

                        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(companyName))
                            continue;

                        var exists = result.Any(x => string.Equals(x.Email.ToLower().Trim(), email.ToLower().Trim(), StringComparison.OrdinalIgnoreCase) &&
                                          string.Equals(x.CompanyName.ToLower().Trim(), companyName.ToLower().Trim(), StringComparison.OrdinalIgnoreCase));

                        if (!exists)
                        {
                            result.Add(new ApplicantDetailModel
                            {
                                Email = email,
                                CompanyName = companyName,
                                PositionName = positionName ?? string.Empty,
                                ResumeLink = resumeLink ?? string.Empty,
                                ApplierName = applierName ?? string.Empty
                            });
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public async Task<MailResult> GenerateMail(List<ApplicantDetailModel> result)
        {
            var mailResult = new MailResult();
            try
            {
                var templateList = await GetTemplateContent();

                foreach (var item in result)
                {
                    mailResult.Total++;

                    // Skip if no email
                    if (string.IsNullOrWhiteSpace(item.Email))
                    {
                        mailResult.Failed++;
                        mailResult.Errors.Add($"No email provided for position {item.PositionName}");
                        continue;
                    }

                    // Fetch template & subject
                    string templateContent = templateList
                        .Where(t => t.Position == item.PositionName)
                        .Select(x => x.TemplateContent)
                        .FirstOrDefault() ?? "";

                    if (string.IsNullOrEmpty(templateContent))
                    {
                        mailResult.Failed++;
                        mailResult.Errors.Add($"No template found for position {item.PositionName}");
                        continue;
                    }


                    string subject = templateList
                        .Where(t => t.Position == item.PositionName)
                        .Select(x => x.SubjectContent)
                        .FirstOrDefault() ?? "";

                    if (string.IsNullOrEmpty(templateContent) || string.IsNullOrEmpty(subject))
                        continue; // Skip if no template found

                    try
                    {

                        // Replace placeholders
                        string emailBody = templateContent
                            .Replace("#CompanyName", item.CompanyName)
                            .Replace("#Position", item.PositionName);

                        string emailSubject = subject
                            .Replace("#CompanyName", item.CompanyName)
                            .Replace("#Position", item.PositionName);

                        await SendMailAysncMethod(emailSubject, emailBody, item.Email, item.ResumeLink, item.ApplierName);
                        mailResult.Success++;
                    }
                    catch (Exception ex)
                    {
                        mailResult.Failed++;
                        mailResult.Errors.Add($"Failed to send email to {item.Email}: {ex.Message}");
                        continue;
                    }
                }

                return mailResult;
            }
            catch (Exception ex)
            {
                throw; // or return ex.Message if you want
            }
        }

        public async Task SendMailAysncMethod(string subject, string body, string toEmail, string googleDriveFileUrl, string ApplicantName)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(_smtpSetting.FromMail, _smtpSetting.FromName);
                    message.To.Add(toEmail);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    // Download file from Google Drive
                    if (!string.IsNullOrWhiteSpace(googleDriveFileUrl))
                    {
                        using (var httpClient = new HttpClient())
                        {
                            // Modify the link to download directly if it's a shareable link
                            var fileBytes = await httpClient.GetByteArrayAsync(googleDriveFileUrl);

                            // Create an attachment from byte array
                            var attachment = new Attachment(new MemoryStream(fileBytes), $"{ApplicantName}_Resume.pdf");

                            message.Attachments.Add(attachment);
                        }
                    }

                    using (var smtp = new SmtpClient(_smtpSetting.Host, _smtpSetting.Port))
                    {
                        smtp.Credentials = new NetworkCredential(_smtpSetting.Username, _smtpSetting.Password);
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(message);

                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error

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
