using Discord;
using Discord.WebSocket;
using SportMania.Handlers.Interface;
using SportMania.Models;
using SportMania.Repository.Interface;
using SportMania.Services.Interface;
using System.Security.Cryptography;

namespace SportMania.Handlers;

public class DiscordCommandHandler : IDiscordCommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DiscordCommandHandler> _logger;

    public DiscordCommandHandler(DiscordSocketClient client, IServiceProvider serviceProvider, ILogger<DiscordCommandHandler> logger)
    {
        _client = client; _serviceProvider = serviceProvider; _logger = logger;
    }

    public async Task HandleSetupRolesAsync(SocketSlashCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        var planRepo = scope.ServiceProvider.GetRequiredService<IPlanRepository>();
        var mappingRepo = scope.ServiceProvider.GetRequiredService<IPlanRoleMappingRepository>();

        if (!await HasAdminPermissionAsync(command))
        {
            await command.RespondAsync("You don't have permission to use this command.", ephemeral: true);
            return;
        }

        var planName = command.Data.Options.First(o => o.Name == "plan").Value.ToString()!;
        var role = (SocketRole)command.Data.Options.First(o => o.Name == "role").Value;

        var plans = await planRepo.GetAllAsync();
        var plan = plans.FirstOrDefault(p => p.Name.Equals(planName, StringComparison.OrdinalIgnoreCase));

        if (plan == null)
        {
            var availablePlans = string.Join(", ", plans.Select(p => $"`{p.Name}`"));
            await command.RespondAsync($"Plan '{planName}' not found.\nAvailable plans: {availablePlans}", ephemeral: true);
            return;
        }

        var existingMapping = await mappingRepo.GetByGuildAndPlanAsync(command.GuildId!.Value, plan.PlanId);
        if (existingMapping != null)
        {
            existingMapping.RoleId = role.Id;
            await mappingRepo.UpdateAsync(existingMapping);
        }
        else
        {
            var mapping = new PlanRoleMapping
            {
                GuildId = command.GuildId!.Value,
                PlanId = plan.PlanId,
                RoleId = role.Id
            };
            await mappingRepo.CreateAsync(mapping);
        }

        await command.RespondAsync($"✅ Mapped **{plan.Name}** → {role.Mention}", ephemeral: true);
    }

    public async Task HandleViewMappingsAsync(SocketSlashCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        var mappingRepo = scope.ServiceProvider.GetRequiredService<IPlanRoleMappingRepository>();

        if (!await HasAdminPermissionAsync(command))
        {
            await command.RespondAsync("You don't have permission to use this command.", ephemeral: true);
            return;
        }

        var mappings = await mappingRepo.GetByGuildIdAsync(command.GuildId!.Value);
        var mappingList = mappings.ToList();

        if (!mappingList.Any())
        {
            await command.RespondAsync("No plan-to-role mappings configured.\nUse `/setuproles` to configure.", ephemeral: true);
            return;
        }

        var guild = _client.GetGuild(command.GuildId!.Value);
        var description = string.Join("\n", mappingList.Select(m =>
        {
            var role = guild.GetRole(m.RoleId);
            return $"**{m.Plan?.Name}** → {role?.Mention ?? "Unknown Role"}";
        }));

        var embed = new EmbedBuilder()
            .WithTitle("🎭 Plan-to-Role Mappings")
            .WithColor(Color.Blue)
            .WithDescription(description)
            .WithFooter($"Total: {mappingList.Count} mappings")
            .Build();

        await command.RespondAsync(embed: embed, ephemeral: true);
    }

    public async Task HandleRemoveMappingAsync(SocketSlashCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        var planRepo = scope.ServiceProvider.GetRequiredService<IPlanRepository>();
        var mappingRepo = scope.ServiceProvider.GetRequiredService<IPlanRoleMappingRepository>();

        if (!await HasAdminPermissionAsync(command))
        {
            await command.RespondAsync("You don't have permission to use this command.", ephemeral: true);
            return;
        }

        var planName = command.Data.Options.First(o => o.Name == "plan").Value.ToString()!;
        var plans = await planRepo.GetAllAsync();
        var plan = plans.FirstOrDefault(p => p.Name.Equals(planName, StringComparison.OrdinalIgnoreCase));

        if (plan == null)
        {
            await command.RespondAsync($"Plan '{planName}' not found.", ephemeral: true);
            return;
        }

        await mappingRepo.DeleteByGuildAndPlanAsync(command.GuildId!.Value, plan.PlanId);
        await command.RespondAsync($"✅ Removed mapping for **{plan.Name}**", ephemeral: true);
    }

    public async Task HandleGenerateAsync(SocketSlashCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        var keyService = scope.ServiceProvider.GetRequiredService<IKeyService>();
        var guildRepo = scope.ServiceProvider.GetRequiredService<IDiscordGuildRepository>();
        var planRepo = scope.ServiceProvider.GetRequiredService<IPlanRepository>();

        if (!await HasAdminPermissionAsync(command))
        {
            await command.RespondAsync("You don't have permission to use this command.", ephemeral: true);
            return;
        }

        var planName = command.Data.Options.First(o => o.Name == "plan").Value.ToString()!;
        var amount = (long?)command.Data.Options.FirstOrDefault(o => o.Name == "amount")?.Value ?? 1;

        var plans = await planRepo.GetAllAsync();
        var plan = plans.FirstOrDefault(p => p.Name.Equals(planName, StringComparison.OrdinalIgnoreCase));

        if (plan == null)
        {
            var availablePlans = string.Join(", ", plans.Select(p => $"`{p.Name}`"));
            await command.RespondAsync($"Plan '{planName}' not found.\nAvailable plans: {availablePlans}", ephemeral: true);
            return;
        }

        if (!int.TryParse(plan.Duration, out int durationDays))
        {
            durationDays = 30;
        }

        var generatedKeys = await keyService.GenerateKeysAsync(command.GuildId!.Value, plan.PlanId, (int)amount, durationDays);

        var embed = new EmbedBuilder()
            .WithTitle($"🔑 {plan.Name} Keys Generated")
            .WithColor(Color.Green)
            .WithDescription($"Generated {amount} key(s) for **{plan.Name}** ({durationDays} days)")
            .AddField("Keys", string.Join("\n", generatedKeys.Select(k => $"`{k.LicenseKey}`")))
            .WithTimestamp(DateTimeOffset.Now)
            .Build();

        await command.RespondAsync(embed: embed, ephemeral: true);
        await LogToChannelAsync(command.GuildId!.Value, guildRepo, $"🔑 {command.User} generated {amount} {plan.Name} key(s)");
    }

    private Task<bool> HasAdminPermissionAsync(SocketSlashCommand command)
    {
        if (command.GuildId == null) return Task.FromResult(false);

        var guild = _client.GetGuild(command.GuildId.Value);
        var user = guild.GetUser(command.User.Id);

        return Task.FromResult(user.GuildPermissions.Administrator || user.GuildPermissions.ManageGuild);
    }

    private async Task LogToChannelAsync(ulong guildId, IDiscordGuildRepository guildRepo, string message)
    {
        var guild = await guildRepo.GetByIdAsync(guildId);
        if (guild?.LogChannelId == null) return;

        var channel = _client.GetChannel(guild.LogChannelId.Value) as ITextChannel;
        if (channel != null)
        {
            await channel.SendMessageAsync(message);
        }
    }
}