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
                Console.WriteLine($"[Client] Submitting form: {data.FirstName} {data.LastName}");

                // Call the API endpoint
                var response = await _httpClient.PostAsJsonAsync("api/wizard/submit", data);

                // Check if the request was successful
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[Client] Server returned error: {response.StatusCode}, {errorMessage}");
                    throw new Exception($"Server returned {response.StatusCode}: {errorMessage}");
                }

                Console.WriteLine("[Client] Form submitted successfully to server");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Client] Error submitting form: {ex.Message}");
                throw;
            }
        }
    }
}
