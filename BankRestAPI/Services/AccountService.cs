using BankRestAPI.Data;
using BankRestAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankRestAPI.Services
{
    public class AccountService : IEntityService<Account>
    {

        private BankDbContext _dbContext;

        public AccountService(BankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // cambiar transfer por account
        public async Task<Account> Create(Account entity)
        {
            await _dbContext.Account.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(Guid id)
        {
            var account = await _dbContext.Account.FindAsync(id);
            _dbContext.Account.Remove(account);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Account>> GetAll()
        {
            return await _dbContext.Account.ToListAsync();
        }

        public async Task<Account> GetById(Guid id)
        {
            var account = await _dbContext.Account.FindAsync(id);
            return account ?? null;
        }

        public async Task<Account> Update(Account entity)
        {
            _dbContext.Account.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
