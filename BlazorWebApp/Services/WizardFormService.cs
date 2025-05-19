using BlazorWebApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebApp.Services
{
    public class WizardFormService : IWizardFormService
    {
        public WizardFormService()
        {
            
        }
        public Task SubmitFormAsync(WizardFormData data)
        {
            // TODO: Persist to database or just log to file/console for demo.
            Console.WriteLine($"Received: {data.FirstName} {data.LastName}");
            return Task.CompletedTask;
        }
    }
}
