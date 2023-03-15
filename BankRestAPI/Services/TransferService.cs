using BankRestAPI.Data;
using BankRestAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankRestAPI.Services
{
    public class TransferService : IEntityService<Transfer>
    {
        private BankDbContext _dbContext;

        public TransferService(BankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Transfer> Create(Transfer entity)
        {
            await _dbContext.Transfer.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(Guid id)
        {
            var transfer = await _dbContext.Transfer.FindAsync(id);
            _dbContext.Transfer.Remove(transfer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Transfer>> GetAll()
        {
            return await _dbContext.Transfer.ToListAsync();
        }

        public async Task<Transfer> GetById(Guid id)
        {
            var transfer = await _dbContext.Transfer.FindAsync(id);
            return transfer ?? null;
        }

        public async Task<Transfer> Update(Transfer entity)
        {
            _dbContext.Transfer.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
