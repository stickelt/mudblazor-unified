using BlazorWebApp.Client.Pages;
using BlazorWebApp.Client.Services;
using BlazorWebApp.Components;
using BlazorWebApp.Services;
using BlazorWebApp.Shared;
using Blazored.LocalStorage;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Add support for API controllers

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents() // components run on the server an the browser ui updates via SignalR/Websockets  (classic blazor server)
    .AddInteractiveWebAssemblyComponents();  // allows parts of the app to render and run on the client ( the the broswer via WASM)
// ABOVE is HOW it allows the Hybrid model.   execute on server for good SEO, fast load and security
// and other things to be on the client



// Register server-side services
builder.Services.AddScoped<IWizardFormService, BlazorWebApp.Services.WizardFormService>();
//  above is scoped -  one instance of the service is created per http request  (  or per user circuit in blazor server)
// shouldn't be shared across all users ( that is a singleton

// Register client-side services for server-side rendering of client components
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<WizardStateService>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["BaseAddress"] ?? builder.Environment.ContentRootPath) });

builder.Services.AddMudServices();
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)})

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Map controllers
app.MapControllers();

// Map Razor components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorWebApp.Client._Imports).Assembly); // using unified model  , server knows about client
   // since server is doing this  pros are easier SSR, pre-rendering  , tighter coupling
   // otherwise the classic server with say you want to swap out the front end

app.Run();
