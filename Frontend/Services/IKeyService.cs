using BlazorApp.Dtos;

namespace BlazorApp.Services;

public interface IKeyService
{
    Task<KeyDto?> GetKeyByIdAsync(Guid keyId);
    Task<KeyDto?> GetKeyByTransactionIdAsync(Guid transactionId);
}
