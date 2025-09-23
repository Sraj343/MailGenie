using MailGenie.Domain.Model.Mail;
using MailGenie.Domain.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace MailGenie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMailService _mailService;
        public MailController(IMailService mailService)
        {
            _mailService = mailService;
        }


        [HttpPost("sendMail")]
        public async Task<IActionResult> SendMail(IFormFile fromFile)
        {
            if (fromFile == null || fromFile.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                var baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
                if (!Directory.Exists(baseDirectory))
                    Directory.CreateDirectory(baseDirectory);

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(fromFile.FileName)}";
                var filePath = Path.Combine(baseDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fromFile.CopyToAsync(stream);
                }

                var mailResult = await _mailService.SendMail(filePath);

                if (mailResult.Failed > 0)
                    return StatusCode(500, mailResult);

                return Ok(mailResult);
            }
            catch (Exception ex)
            {
                var errorResult = new MailResult
                {
                    Total = 0,
                    Success = 0,
                    Failed = 0,
                    Errors = new List<string> { ex.Message }
                };
                return StatusCode(500, errorResult);
            }
        }

        [HttpPost("AddTemplate")]
        public async Task<IActionResult> AddEmailTemplate([FromBody] EmailTemplate template)
        {
            if (template == null)
                return BadRequest("Template cannot be null.");

            try
            {
                var result = await _mailService.AddEmailTemplate(template); // Pass the template
                if (result)
                    return Ok(new { message = "Template added successfully." });
                else
                    return BadRequest("Failed to add template.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
