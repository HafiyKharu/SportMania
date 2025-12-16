using SportMania.Models;
using System;
using System.Threading.Tasks;

namespace SportMania.Repository.Interface
{
    public interface IKeyRepository
    {
        Task<Key> CreateAsync(Key key);
        Task<Key?> GetByIdAsync(Guid id);
    }
}