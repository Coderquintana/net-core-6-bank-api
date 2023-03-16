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

        public async Task<Transfer?> Create(Transfer entity)
        {
            await _dbContext.Transfer.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(Guid id)
        {
            var transfer = await _dbContext.Transfer.FindAsync(id);
            if (transfer != null)
            {
                _dbContext.Transfer.Remove(transfer);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Transfer?>> GetAll()
        {
            return await _dbContext.Transfer
                .Include(t => t.FromBank)
                .Include(t => t.ToBank)
                .Include(t => t.FromAccount)
                .Include(t => t.ToAccount)
                .Include(t => t.ToCustomer)
                .Include(t => t.FromCustomer)
                .ToListAsync();
        }

        public async Task<Transfer?> GetById(Guid id)
        {
            var transfer = await _dbContext.Transfer
                .Include(t => t.FromBank)
                .Include(t => t.ToBank)
                .Include(t => t.FromAccount)
                .Include(t => t.ToAccount)
                .Include (t => t.ToCustomer)
                .Include(t => t.FromCustomer)
                .FirstOrDefaultAsync(t => t.Id == id);
            return transfer;
        }

        public async Task<Transfer?> Update(Transfer entity)
        {
            _dbContext.Transfer.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<Transfer>> FindByAccount(string accountNumber)
        {
            var send = await _dbContext.Transfer.Where(t => t.FromAccount.Number == accountNumber)
                .Include(t => t.FromBank)
                .Include(t => t.ToBank)
                .Include(t => t.FromAccount)
                .Include(t => t.ToAccount)
                .Include(t => t.ToCustomer)
                .Include(t => t.FromCustomer)
                .ToListAsync();
            var receive = await _dbContext.Transfer.Where(t => t.ToAccount.Number == accountNumber)
                .Include(t => t.FromBank)
                .Include(t => t.ToBank)
                .Include(t => t.FromAccount)
                .Include(t => t.ToAccount)
                .Include(t => t.ToCustomer)
                .Include(t => t.FromCustomer)
                .ToListAsync();
            return receive.Concat(send);
        }

        public async Task<IEnumerable<Transfer>> FindByCustomer(string documentNumber)
        {
            var send = await _dbContext.Transfer.Where(t => t.FromCustomer.DocumentNumber == documentNumber)
                .Include(t => t.FromBank)
                .Include(t => t.ToBank)
                .Include(t => t.FromAccount)
                .Include(t => t.ToAccount)
                .Include(t => t.ToCustomer)
                .Include(t => t.FromCustomer)
                .ToListAsync();
            var receive = await _dbContext.Transfer.Where(t => t.ToCustomer.DocumentNumber == documentNumber)
                .Include(t => t.FromBank)
                .Include(t => t.ToBank)
                .Include(t => t.FromAccount)
                .Include(t => t.ToAccount)
                .Include(t => t.ToCustomer)
                .Include(t => t.FromCustomer)
                .ToListAsync();

            return receive.Concat(send);
        }

    }
}
