using BlazorWebApp.Shared;
using System.Net.Http.Json;

namespace BlazorWebApp.Client
{
    public class WizardFormServiceProxy : IWizardFormService
    {
        private readonly HttpClient _httpClient;
        public WizardFormServiceProxy(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task SubmitFormAsync(WizardFormData data)
        {
            var response = await _httpClient.PostAsJsonAsync("api/wizard", data);
            response.EnsureSuccessStatusCode();
        }
    }
}
