namespace SportMania.Services.Interface;

public interface IDiscordBotService
{
    Task StartAsync();
    Task StopAsync();
}