using Discord;
using Discord.WebSocket;
using SportMania.Services.Interface;
using SportMania.Handlers.Interface;

namespace SportMania.Services;

public class DiscordBotService : IDiscordBotService, IHostedService
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DiscordBotService> _logger;

    public DiscordBotService(
        DiscordSocketClient client,
        IConfiguration configuration,
        IServiceProvider serviceProvider,
        ILogger<DiscordBotService> logger)
    {
        _client = client;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        _logger = logger;

        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _client.SlashCommandExecuted += SlashCommandExecutedAsync;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        var token = _configuration["Discord:BotToken"]
            ?? throw new InvalidOperationException("Discord bot token not configured.");

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        await _client.StopAsync();
    }

    Task IDiscordBotService.StartAsync() => StartAsync();
    Task IDiscordBotService.StopAsync() => StopAsync();

    private Task LogAsync(LogMessage log)
    {
        _logger.LogInformation("{Message}", log.ToString());
        return Task.CompletedTask;
    }

    private async Task ReadyAsync()
    {
        _logger.LogInformation("🤖 Discord bot is ready. Connected as {User}", _client.CurrentUser);

        var commands = new List<SlashCommandBuilder>
        {
            new SlashCommandBuilder()
                .WithName("setuproles")
                .WithDescription("Set up plan-to-role mapping")
                .AddOption("plan", ApplicationCommandOptionType.String, "Plan name", isRequired: true)
                .AddOption("role", ApplicationCommandOptionType.Role, "Discord role", isRequired: true),

            new SlashCommandBuilder()
                .WithName("viewmappings")
                .WithDescription("View all plan-to-role mappings"),

            new SlashCommandBuilder()
                .WithName("removemapping")
                .WithDescription("Remove a plan-to-role mapping")
                .AddOption("plan", ApplicationCommandOptionType.String, "Plan name", isRequired: true),

            new SlashCommandBuilder()
                .WithName("generate")
                .WithDescription("Generate license keys")
                .AddOption("plan", ApplicationCommandOptionType.String, "Plan name", isRequired: true)
                .AddOption("amount", ApplicationCommandOptionType.Integer, "Number of keys", isRequired: false),

            new SlashCommandBuilder()
                .WithName("redeem")
                .WithDescription("Redeem a license key")
                .AddOption("key", ApplicationCommandOptionType.String, "License key", isRequired: true)
        };

        _logger.LogInformation("⏳ Waiting 2 seconds before registering commands...");
        await Task.Delay(2000);

        foreach (var guild in _client.Guilds)
        {
            try
            {
                _logger.LogInformation("🔄 Processing guild: {GuildName} ({GuildId})", guild.Name, guild.Id);

                // Delete old commands
                var existingCommands = await guild.GetApplicationCommandsAsync();
                _logger.LogInformation("📋 Found {Count} existing commands", existingCommands.Count);

                foreach (var cmd in existingCommands)
                {
                    await cmd.DeleteAsync();
                    _logger.LogInformation("🗑️ Deleted: {CommandName}", cmd.Name);
                }

                await Task.Delay(1000);

                // Register new commands
                foreach (var command in commands)
                {
                    await guild.CreateApplicationCommandAsync(command.Build());
                    _logger.LogInformation("✅ Registered: {CommandName}", command.Name);
                }

                _logger.LogInformation("🎉 Successfully registered {Count} commands for {GuildName}", commands.Count, guild.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to register commands for guild {GuildId}", guild.Id);
            }
        }

        _logger.LogInformation("✨ Command registration complete!");
    }

    private async Task SlashCommandExecutedAsync(SocketSlashCommand command)
    {
        _logger.LogInformation("⚡ Command received: {CommandName} from {User}", command.CommandName, command.User);

        try
        {
            // Create a scope to get scoped services
            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IDiscordCommandHandler>();

            await (command.CommandName switch
            {
                "setuproles" => handler.HandleSetupRolesAsync(command),
                "viewmappings" => handler.HandleViewMappingsAsync(command),
                "removemapping" => handler.HandleRemoveMappingAsync(command),
                "generate" => handler.HandleGenerateAsync(command),
                _ => command.RespondAsync("❌ Unknown command.", ephemeral: true)
            });

            _logger.LogInformation("✅ Command completed: {CommandName}", command.CommandName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error executing command {Command}", command.CommandName);

            if (!command.HasResponded)
            {
                await command.RespondAsync("An error occurred while processing the command.", ephemeral: true);
            }
        }
    }
}