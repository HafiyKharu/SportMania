using SportMania.Models;
using SportMania.Repository.Interface;
using SportMania.Services.Interface;
using System.Text;

namespace SportMania.Services;

public class KeyService : IKeyService
{
    private const int KeyLength = 16;
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    
    /// <summary>
    /// Generates a new unique key and saves it to the database.
    /// </summary>
    /// <returns>The newly created Key object.</returns>
    public async Task<Key> GenerateKeyAsync()
    {
        var newKey = new Key
        {
            KeyId = Guid.NewGuid(),
            Code = GenerateUniqueCode(),
            IsRedeemed = false
        };

        return await Task.FromResult(newKey);
    }

    private static string GenerateUniqueCode()
    {
        var random = new Random();
        var randomString = new StringBuilder(KeyLength);
        for (int i = 0; i < KeyLength; i++)
        {
            randomString.Append(Chars[random.Next(Chars.Length)]);
        }
        return randomString.ToString();
    }
}