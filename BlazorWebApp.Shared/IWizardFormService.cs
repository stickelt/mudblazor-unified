using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebApp.Shared
{
    public interface IWizardFormService
    {
        Task SubmitFormAsync(WizardFormData data);
        //Task<WizardFormData> GetFormAsync(Guid id);// (if you want to fetch a saved form by ID)

        //Task<bool> ValidateFormAsync(WizardFormData data); // (for server-side validation)
    }
}
