using Discord;
using Discord.WebSocket;
using SportMania.Handlers.Interface;
using SportMania.Models;
using SportMania.Repository.Interface;
using System.Security.Cryptography;

namespace SportMania.Handlers;

public class DiscordCommandHandler : IDiscordCommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DiscordCommandHandler> _logger;

    public DiscordCommandHandler(
        DiscordSocketClient client,
        IServiceProvider serviceProvider,
        ILogger<DiscordCommandHandler> logger)
    {
        _client = client;
        _serviceProvider = serviceProvider;
        _logger = logger;
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
        var keyRepo = scope.ServiceProvider.GetRequiredService<IKeyRepository>();
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

        var keys = new List<string>();
        for (int i = 0; i < amount; i++)
        {
            var licenseKey = GenerateLicenseKey();
            var key = new Key
            {
                LicenseKey = licenseKey,
                GuildId = command.GuildId!.Value,
                PlanId = plan.PlanId,
                DurationDays = durationDays,
                IsActive = true
            };
            await keyRepo.CreateAsync(key);
            keys.Add(licenseKey);
        }

        var embed = new EmbedBuilder()
            .WithTitle($"🔑 {plan.Name} Keys Generated")
            .WithColor(Color.Green)
            .WithDescription($"Generated {amount} key(s) for **{plan.Name}** ({durationDays} days)")
            .AddField("Keys", string.Join("\n", keys.Select(k => $"`{k}`")))
            .WithTimestamp(DateTimeOffset.Now)
            .Build();

        await command.RespondAsync(embed: embed, ephemeral: true);
        await LogToChannelAsync(command.GuildId!.Value, guildRepo, $"🔑 {command.User} generated {amount} {plan.Name} key(s)");
    }

    public async Task HandleRedeemAsync(SocketSlashCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        var keyRepo = scope.ServiceProvider.GetRequiredService<IKeyRepository>();
        var guildRepo = scope.ServiceProvider.GetRequiredService<IDiscordGuildRepository>();
        var mappingRepo = scope.ServiceProvider.GetRequiredService<IPlanRoleMappingRepository>();

        var licenseKey = command.Data.Options.First(o => o.Name == "key").Value.ToString()!;
        var guildId = command.GuildId!.Value;

        var existingKey = await keyRepo.GetByUserIdAndGuildAsync(command.User.Id, guildId);
        if (existingKey != null && existingKey.IsActive)
        {
            await command.RespondAsync("You already have an active license!", ephemeral: true);
            return;
        }

        var key = await keyRepo.GetByLicenseKeyAndGuildAsync(licenseKey, guildId);
        if (key == null)
        {
            await command.RespondAsync("Invalid license key.", ephemeral: true);
            return;
        }

        if (key.RedeemedByUserId != null)
        {
            await command.RespondAsync("This key has already been redeemed.", ephemeral: true);
            return;
        }

        key.RedeemedByUserId = command.User.Id;
        key.RedeemedAt = DateTime.UtcNow;
        key.ExpiresAt = DateTime.UtcNow.AddDays(key.DurationDays);
        await keyRepo.UpdateAsync(key);

        var mapping = await mappingRepo.GetByGuildAndPlanAsync(guildId, key.PlanId);
        if (mapping != null)
        {
            var socketGuild = _client.GetGuild(guildId);
            var user = socketGuild.GetUser(command.User.Id);
            var role = socketGuild.GetRole(mapping.RoleId);

            if (role != null && user != null)
            {
                await user.AddRoleAsync(role);
            }
        }

        var embed = new EmbedBuilder()
            .WithTitle("✅ License Activated!")
            .WithColor(Color.Green)
            .AddField("Plan", key.Plan?.Name ?? "Unknown", inline: true)
            .AddField("Duration", $"{key.DurationDays} days", inline: true)
            .AddField("Expires", key.ExpiresAt?.ToString("yyyy-MM-dd HH:mm UTC") ?? "Never")
            .WithFooter("Thank you for your purchase!")
            .WithTimestamp(DateTimeOffset.Now)
            .Build();

        await command.RespondAsync(embed: embed, ephemeral: true);
        await LogToChannelAsync(guildId, guildRepo, $"✅ {command.User.Mention} activated {key.Plan?.Name} (expires: {key.ExpiresAt:yyyy-MM-dd})");
    }

    public async Task HandleSetLogAsync(SocketSlashCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        var guildRepo = scope.ServiceProvider.GetRequiredService<IDiscordGuildRepository>();

        if (!await HasAdminPermissionAsync(command))
        {
            await command.RespondAsync("You don't have permission to use this command.", ephemeral: true);
            return;
        }

        var channel = (SocketChannel)command.Data.Options.First(o => o.Name == "channel").Value;
        var guild = await guildRepo.GetOrCreateAsync(command.GuildId!.Value);
        guild.LogChannelId = channel.Id;
        await guildRepo.UpdateAsync(guild);

        await command.RespondAsync($"Log channel set to <#{channel.Id}>", ephemeral: true);
    }

    public async Task HandleKeysAsync(SocketSlashCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        var keyRepo = scope.ServiceProvider.GetRequiredService<IKeyRepository>();

        if (!await HasAdminPermissionAsync(command))
        {
            await command.RespondAsync("You don't have permission to use this command.", ephemeral: true);
            return;
        }

        var keys = await keyRepo.GetActiveKeysByGuildIdAsync(command.GuildId!.Value);
        var keyList = keys.ToList();

        if (!keyList.Any())
        {
            await command.RespondAsync("No unredeemed keys found.", ephemeral: true);
            return;
        }

        var embed = new EmbedBuilder()
            .WithTitle("🔑 Unredeemed License Keys")
            .WithColor(Color.Blue)
            .WithDescription(string.Join("\n", keyList.Select(k =>
                $"`{k.LicenseKey}` - **{k.Plan?.Name ?? "Unknown"}** ({k.DurationDays} days)")))
            .WithFooter($"Total: {keyList.Count} keys")
            .WithTimestamp(DateTimeOffset.Now)
            .Build();

        await command.RespondAsync(embed: embed, ephemeral: true);
    }

    public async Task HandleDeleteAsync(SocketSlashCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        var keyRepo = scope.ServiceProvider.GetRequiredService<IKeyRepository>();

        if (!await HasAdminPermissionAsync(command))
        {
            await command.RespondAsync("You don't have permission to use this command.", ephemeral: true);
            return;
        }

        var licenseKey = command.Data.Options.First(o => o.Name == "key").Value.ToString()!;
        var key = await keyRepo.GetByLicenseKeyAndGuildAsync(licenseKey, command.GuildId!.Value);

        if (key == null)
        {
            await command.RespondAsync("Key not found.", ephemeral: true);
            return;
        }

        await keyRepo.DeleteAsync(key.KeyId);
        await command.RespondAsync($"Key `{licenseKey}` has been deleted.", ephemeral: true);
    }

    public async Task HandleStatusAsync(SocketSlashCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        var keyRepo = scope.ServiceProvider.GetRequiredService<IKeyRepository>();

        var key = await keyRepo.GetByUserIdAndGuildAsync(command.User.Id, command.GuildId!.Value);

        if (key == null)
        {
            await command.RespondAsync("You don't have an active license.", ephemeral: true);
            return;
        }

        var embed = new EmbedBuilder()
            .WithTitle("📋 License Status")
            .WithColor(Color.Blue)
            .AddField("Plan", key.Plan?.Name ?? "Unknown", inline: true)
            .AddField("Status", key.IsActive ? "✅ Active" : "❌ Inactive", inline: true)
            .AddField("Redeemed", key.RedeemedAt?.ToString("yyyy-MM-dd") ?? "N/A", inline: true)
            .AddField("Expires", key.ExpiresAt?.ToString("yyyy-MM-dd HH:mm UTC") ?? "Never", inline: true)
            .WithTimestamp(DateTimeOffset.Now)
            .Build();

        await command.RespondAsync(embed: embed, ephemeral: true);
    }

    public async Task HandleRevokeAsync(SocketSlashCommand command)
    {
        using var scope = _serviceProvider.CreateScope();
        var keyRepo = scope.ServiceProvider.GetRequiredService<IKeyRepository>();
        var guildRepo = scope.ServiceProvider.GetRequiredService<IDiscordGuildRepository>();
        var mappingRepo = scope.ServiceProvider.GetRequiredService<IPlanRoleMappingRepository>();

        if (!await HasAdminPermissionAsync(command))
        {
            await command.RespondAsync("You don't have permission to use this command.", ephemeral: true);
            return;
        }

        var user = (SocketUser)command.Data.Options.First(o => o.Name == "user").Value;
        var guildId = command.GuildId!.Value;
        var key = await keyRepo.GetByUserIdAndGuildAsync(user.Id, guildId);

        if (key == null)
        {
            await command.RespondAsync("This user doesn't have an active license.", ephemeral: true);
            return;
        }

        key.IsActive = false;
        await keyRepo.UpdateAsync(key);

        var mapping = await mappingRepo.GetByGuildAndPlanAsync(guildId, key.PlanId);
        if (mapping != null)
        {
            var socketGuild = _client.GetGuild(guildId);
            var role = socketGuild.GetRole(mapping.RoleId);
            var guildUser = socketGuild.GetUser(user.Id);
            if (role != null && guildUser != null)
            {
                await guildUser.RemoveRoleAsync(role);
            }
        }

        await command.RespondAsync($"License revoked from {user.Mention}", ephemeral: true);
        await LogToChannelAsync(guildId, guildRepo, $"❌ {command.User} revoked license from {user}");
    }

    private static string GenerateLicenseKey()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var segments = new string[4];

        for (int i = 0; i < 4; i++)
        {
            var segment = new char[5];
            for (int j = 0; j < 5; j++)
            {
                segment[j] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
            }
            segments[i] = new string(segment);
        }

        return string.Join("-", segments);
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