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
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] WizardFormData data)
        {
            await _service.SubmitFormAsync(data);
            return Ok();
        }
    }
}
