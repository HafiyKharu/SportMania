using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportMania.Data;
using SportMania.Repository.Interface;
using SportMania.Repository;
using SportMania.Services.Interface;
using SportMania.Services;
using SportMania.Handlers.Interface;
using SportMania.Handlers;
using Discord;
using Discord.WebSocket;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

// Add database connection.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add Controllers
builder.Services.AddControllersWithViews();

// Add repositories
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IPlanDetailsRepository, PlanDetailsRepository>();
builder.Services.AddScoped<IKeyRepository, KeyRepository>();
builder.Services.AddScoped<IDiscordGuildRepository, DiscordGuildRepository>();
builder.Services.AddScoped<IPlanRoleMappingRepository, PlanRoleMappingRepository>();

// Add handlers
builder.Services.AddScoped<IToyyibPayHandler, ToyyibPayHandler>();
builder.Services.AddScoped<IDiscordCommandHandler, DiscordCommandHandler>(); // ADD THIS LINE

// Add services
builder.Services.AddScoped<IKeyService, KeyService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

// Register DiscordSocketClient as singleton
builder.Services.AddSingleton<DiscordSocketClient>(provider =>
{
    var config = new DiscordSocketConfig
    {
        GatewayIntents = GatewayIntents.Guilds |
                        GatewayIntents.GuildMessages |
                        GatewayIntents.MessageContent |
                        GatewayIntents.GuildMembers
    };
    return new DiscordSocketClient(config);
});

// Register bot service as singleton
builder.Services.AddSingleton<IDiscordBotService, DiscordBotService>();
builder.Services.AddHostedService(provider => (DiscordBotService)provider.GetRequiredService<IDiscordBotService>());

// Add hosted service for license expiration
builder.Services.AddHostedService<LicenseExpirationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
