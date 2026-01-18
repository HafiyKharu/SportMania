using Discord.WebSocket;

namespace SportMania.Handlers.Interface;

public interface IDiscordCommandHandler
{
    Task HandleSetupRolesAsync(SocketSlashCommand command);
    Task HandleViewMappingsAsync(SocketSlashCommand command);
    Task HandleRemoveMappingAsync(SocketSlashCommand command);
    Task HandleGenerateAsync(SocketSlashCommand command);
    Task HandleRedeemAsync(SocketSlashCommand command);
    Task HandleSetLogAsync(SocketSlashCommand command);
    Task HandleKeysAsync(SocketSlashCommand command);
    Task HandleDeleteAsync(SocketSlashCommand command);
    Task HandleStatusAsync(SocketSlashCommand command);
    Task HandleRevokeAsync(SocketSlashCommand command);
}