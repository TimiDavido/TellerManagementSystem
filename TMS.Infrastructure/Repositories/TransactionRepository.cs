using Microsoft.EntityFrameworkCore;
using TMS.Application.Interfaces;
using TMS.Domain.Entities;
using TMS.Domain.Enums;
using TMS.Infrastructure.Data;

namespace TMS.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Transaction>> GetAllTransactionsAsync(TransactionType? type, DateTime? fromUtc, DateTime? toUtc)
        {
            var query = _context.Transactions.AsNoTracking().AsQueryable();

            if (type.HasValue)
                query = query.Where(t => t.TransactionType == type.Value);

            if (fromUtc.HasValue)
                query = query.Where(t => t.CreatedAt >= fromUtc.Value);

            if (toUtc.HasValue)
                query = query.Where(t => t.CreatedAt < toUtc.Value);

            return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
        }

        public async Task<Transaction?> GetTransactionByIdAsync(Guid transactionId)
        {
            return await _context.Transactions.AsNoTracking().FirstOrDefaultAsync(t => t.Id == transactionId);
        }

        public async Task<List<Transaction>> GetTransactionsByTellerId(Guid tellerId)
        {
            return await _context.Transactions.AsNoTracking()
                .Where(t => t.TellerId == tellerId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Transaction>> GetTransactionsByTellerIdAsync(Guid tellerId)
        {
            return await _context.Transactions
                .AsNoTracking()
                .Where(t => t.TellerId == tellerId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}
