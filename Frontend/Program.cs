using BlazorApp.Components;
using BlazorApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(options =>
    {
        options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
    });


// Add HttpClient for API communication
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5235";
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});

// Add Blazor services
builder.Services.AddScoped<IPlanService>(sp => 
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    var environment = sp.GetRequiredService<IWebHostEnvironment>();
    return new PlanService(httpClient, environment);
});
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IKeyService, KeyService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();