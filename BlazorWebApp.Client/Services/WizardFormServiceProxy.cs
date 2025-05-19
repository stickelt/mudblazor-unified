using BlazorWebApp.Shared;
using System.Net.Http.Json;

namespace BlazorWebApp.Client.Services
{
    /// <summary>
    /// Client-side proxy for the WizardFormService that forwards requests to the server
    /// </summary>
    public class WizardFormServiceProxy : IWizardFormService
    {
        private readonly HttpClient _httpClient;

        public WizardFormServiceProxy(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SubmitFormAsync(WizardFormData data)
        {
            try
            {
                // In a real application, this would call an API endpoint
                // For now, we'll just log to the console
                Console.WriteLine($"[Client] Submitting form: {data.FirstName} {data.LastName}");
                
                // Uncomment this when you have an API endpoint to call
                // await _httpClient.PostAsJsonAsync("api/wizard/submit", data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error submitting form: {ex.Message}");
                throw;
            }
        }
    }
}
