using MailGenie.Domain.Model.EmailTemplateModel;
using MailGenie.Domain.Model.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MailGenie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailTemplateController : ControllerBase
    {

        private readonly IEmailTemplateService _mailService;

        public EmailTemplateController(IEmailTemplateService mailService)
        {
            _mailService = mailService;
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


        [HttpGet("GetTemplate")]
        public async Task<IActionResult> GetTemplate()
        {
            try
            {
                var result = await _mailService.GetTemplateContent();

                if (result == null || !result.Any())
                    return NotFound(new { Status = "Error", Message = "No templates found." });

                var response = new
                {
                    Status = "Success",
                    Data = result
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = $"Internal server error: {ex.Message}" });
            }
        }

    }
}
