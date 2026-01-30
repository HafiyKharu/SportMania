using Discord.WebSocket;

namespace SportMania.Handlers.Interface;

public interface IDiscordBotHandlers
{
    Task HandleSetupRolesAsync(SocketSlashCommand command);
    Task HandleViewMappingsAsync(SocketSlashCommand command);
    Task HandleRemoveMappingAsync(SocketSlashCommand command);
    Task HandleGenerateAsync(SocketSlashCommand command);
    Task HandleRedeemAsync(SocketSlashCommand command);
}