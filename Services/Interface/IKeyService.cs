using SportMania.Models;

namespace SportMania.Services.Interface;

public interface IKeyService
{
    Task<Key> GenerateKeyAsync();
}