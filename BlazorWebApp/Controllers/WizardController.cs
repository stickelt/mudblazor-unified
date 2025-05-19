using BlazorWebApp.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BlazorWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WizardController : ControllerBase
    {
        private readonly IWizardFormService _service;
        private readonly ILogger<WizardController> _logger;

        public WizardController(IWizardFormService service, ILogger<WizardController> logger)
        {
            _service = service;
            _logger = logger;
        }
        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] WizardFormData data)
        {
            try
            {
                // Add very visible logging to confirm the controller method was called
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("*******************************************************************");
                Console.WriteLine($"*** SERVER CONTROLLER: Received form submission at {DateTime.Now} ***");
                Console.WriteLine($"*** First Name: {data.FirstName}");
                Console.WriteLine($"*** Last Name: {data.LastName}");
                Console.WriteLine($"*** Request Path: {HttpContext.Request.Path}");
                Console.WriteLine($"*** Request Method: {HttpContext.Request.Method}");
                Console.WriteLine($"*** Content Type: {HttpContext.Request.ContentType}");
                Console.WriteLine($"*** User Agent: {HttpContext.Request.Headers["User-Agent"]}");
                Console.WriteLine("*******************************************************************");
                Console.ResetColor();

                // Also log to the server's logger
                _logger.LogInformation("SERVER CONTROLLER CALLED: Received form submission");
                _logger.LogInformation("First Name: {FirstName}, Last Name: {LastName}", data.FirstName, data.LastName);
                _logger.LogInformation("Request Path: {Path}, Method: {Method}", HttpContext.Request.Path, HttpContext.Request.Method);

                // Process the form data
                await _service.SubmitFormAsync(data);

                // Return a response with a unique identifier to verify in the client
                var responseId = Guid.NewGuid().ToString("N").Substring(0, 8);
                var response = new {
                    message = "Form submitted successfully",
                    timestamp = DateTime.Now,
                    responseId = responseId,
                    serverInfo = Environment.MachineName,
                    controllerName = "WizardController",
                    methodName = "Submit"
                };

                // Log the response
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("*******************************************************************");
                Console.WriteLine($"*** SERVER CONTROLLER: Sending response at {DateTime.Now} ***");
                Console.WriteLine($"*** Response ID: {responseId}");
                Console.WriteLine($"*** Response: {System.Text.Json.JsonSerializer.Serialize(response)}");
                Console.WriteLine("*******************************************************************");
                Console.ResetColor();

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("*******************************************************************");
                Console.WriteLine($"*** SERVER CONTROLLER ERROR: {ex.Message}");
                Console.WriteLine($"*** Exception Type: {ex.GetType().Name}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"*** Inner Exception: {ex.InnerException.Message}");
                }
                Console.WriteLine($"*** Stack Trace: {ex.StackTrace}");
                Console.WriteLine("*******************************************************************");
                Console.ResetColor();

                return StatusCode(500, new { error = "An error occurred while processing the form", details = ex.Message });
            }
        }
    }
}
