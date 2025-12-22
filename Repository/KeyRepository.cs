using Microsoft.EntityFrameworkCore;
using SportMania.Data;
using SportMania.Models;
using SportMania.Repository.Interface;
using System;
using System.Threading.Tasks;

namespace SportMania.Repository
{
    public class KeyRepository : IKeyRepository
    {
        private readonly ApplicationDbContext _context;

        public KeyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Key> CreateAsync(Key key)
        {
            await _context.Keys.AddAsync(key);
            await _context.SaveChangesAsync();
            return key;
        }

        public async Task<Key?> GetByIdAsync(Guid id)
        {
            return await _context.Keys.FirstOrDefaultAsync(k => k.KeyId == id);
        }
    }
}