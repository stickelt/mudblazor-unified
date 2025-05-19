using BlazorWebApp.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WizardController : ControllerBase
    {
        private readonly IWizardFormService _service;
        public WizardController(IWizardFormService service)
        {
            _service = service;
        }
        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] WizardFormData data)
        {
            try
            {
                // Log the incoming request
                Console.WriteLine($"[Server] Received form submission: {data.FirstName} {data.LastName}");

                // Process the form data
                await _service.SubmitFormAsync(data);

                // Return success response
                return Ok(new { message = "Form submitted successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Server] Error processing form: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the form");
            }
        }
    }
}
