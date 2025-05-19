using BlazorWebApp.Shared;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace BlazorWebApp.Client.Services
{
    /// <summary>
    /// Service for managing wizard form state across components and page refreshes
    /// </summary>
    public class WizardStateService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;
        private const string WIZARD_DATA_KEY = "wizardData";
        private const string WIZARD_STEP_KEY = "wizardStep";

        public WizardFormData FormData { get; set; } = new();
        public int CurrentStep { get; set; } = 0;

        // Event that components can subscribe to for state changes
        public event Action? StateChanged;

        public WizardStateService(ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            _localStorage = localStorage;
            _navigationManager = navigationManager;
        }

        /// <summary>
        /// Initializes the state from localStorage if available
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                // Load saved step
                var savedStep = await _localStorage.GetItemAsync<int>(WIZARD_STEP_KEY);
                if (savedStep > 0)
                    CurrentStep = savedStep;

                // Load saved form data
                var savedData = await _localStorage.GetItemAsync<WizardFormData>(WIZARD_DATA_KEY);
                if (savedData != null)
                    FormData = savedData;
            }
            catch (Exception)
            {
                // If there's an error loading from localStorage, use default values
                CurrentStep = 0;
                FormData = new WizardFormData();
            }
        }

        /// <summary>
        /// Updates the form data and persists it to localStorage
        /// </summary>
        public async Task UpdateFormDataAsync(WizardFormData formData)
        {
            FormData = formData;
            await SaveStateAsync();
            NotifyStateChanged();
        }

        /// <summary>
        /// Updates a specific property of the form data
        /// </summary>
        public async Task UpdateFormPropertyAsync<T>(string propertyName, T value)
        {
            var property = typeof(WizardFormData).GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(FormData, value);
                await SaveStateAsync();
                NotifyStateChanged();
            }
        }

        /// <summary>
        /// Moves to the next step and saves state
        /// </summary>
        public async Task NextStepAsync()
        {
            CurrentStep++;
            await SaveStateAsync();
            NotifyStateChanged();
        }

        /// <summary>
        /// Moves to the previous step and saves state
        /// </summary>
        public async Task PreviousStepAsync()
        {
            if (CurrentStep > 0)
            {
                CurrentStep--;
                await SaveStateAsync();
                NotifyStateChanged();
            }
        }

        /// <summary>
        /// Sets the current step directly
        /// </summary>
        public async Task SetStepAsync(int step)
        {
            if (step >= 0)
            {
                CurrentStep = step;
                await SaveStateAsync();
                NotifyStateChanged();
            }
        }

        /// <summary>
        /// Saves the current state to localStorage
        /// </summary>
        private async Task SaveStateAsync()
        {
            await _localStorage.SetItemAsync(WIZARD_STEP_KEY, CurrentStep);
            await _localStorage.SetItemAsync(WIZARD_DATA_KEY, FormData);
        }

        /// <summary>
        /// Clears the saved state (e.g., after successful submission)
        /// </summary>
        public async Task ClearStateAsync()
        {
            await _localStorage.RemoveItemAsync(WIZARD_STEP_KEY);
            await _localStorage.RemoveItemAsync(WIZARD_DATA_KEY);
            FormData = new WizardFormData();
            CurrentStep = 0;
            NotifyStateChanged();
        }

        /// <summary>
        /// Notifies subscribers that the state has changed
        /// </summary>
        private void NotifyStateChanged() => StateChanged?.Invoke();
    }
}
