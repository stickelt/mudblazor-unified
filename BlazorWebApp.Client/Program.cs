using BlazorWebApp.Client;
using BlazorWebApp.Client.Services;
using BlazorWebApp.Shared;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped<IWizardFormService, BlazorWebApp.Client.Services.WizardFormServiceProxy>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<WizardStateService>();

// Register the Service Bus client service
builder.Services.AddScoped<BlazorWebApp.Client.Services.ServiceBusClientService>();

await builder.Build().RunAsync();
