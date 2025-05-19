using BlazorWebApp.Shared;

namespace BlazorWebApp.Services
{
    /// <summary>
    /// Server-side implementation of the WizardFormService
    /// This is where actual business logic and data persistence should happen
    /// </summary>
    public class WizardFormService : IWizardFormService
    {
        private readonly ILogger<WizardFormService> _logger;

        public WizardFormService(ILogger<WizardFormService> logger)
        {
            _logger = logger;
        }

        public Task SubmitFormAsync(WizardFormData data)
        {
            // In a real application, this would save to a database
            _logger.LogInformation("Form submitted: {FirstName} {LastName}", data.FirstName, data.LastName);

            // Add very visible console logging
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("*******************************************************************");
            Console.WriteLine($"*** SERVER SERVICE: Processing form data at {DateTime.Now} ***");
            Console.WriteLine($"*** First Name: {data.FirstName}");
            Console.WriteLine($"*** Last Name: {data.LastName}");
            Console.WriteLine("*** In a real app, this would be saved to a database");
            Console.WriteLine("*******************************************************************");
            Console.ResetColor();

            // TODO: Add database persistence, email notifications, etc.

            return Task.CompletedTask;
        }
    }
}
