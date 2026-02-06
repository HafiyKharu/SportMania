using SportMania.Repository.Interface;

namespace SportMania.Services;

public class LicenseExpirationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LicenseExpirationService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

    public LicenseExpirationService(
        IServiceProvider serviceProvider,
        ILogger<LicenseExpirationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckExpiredLicenses();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking expired licenses");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task CheckExpiredLicenses()
    {
        using var scope = _serviceProvider.CreateScope();
        var keyRepo = scope.ServiceProvider.GetRequiredService<IKeyRepository>();
        
        await keyRepo.DeleteExpiredKeysAsync();
        _logger.LogInformation("Expired licenses cleanup completed");
    }
}