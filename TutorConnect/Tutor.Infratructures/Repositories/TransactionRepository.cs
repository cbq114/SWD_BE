using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Configurations;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.PaymentModel;
using Tutor.Infratructures.Persistence;
using Tutor.Shared.Helper;

namespace Tutor.Infratructures.Repositories
{
    public class TransactionRepository : Repository<Transactions>, ITransactionRepository
    {
        private readonly TutorDBContext _dbContext;
        private readonly IMapper _mapper;
        public TransactionRepository(TutorDBContext dbContext, IMapper mapper) : base(dbContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<TransactionDTO>> GetAllTransactions()
        {
            var transactions = await Entities
                .Include(t => t.Wallet)
                .ToListAsync();

            return _mapper.Map<List<TransactionDTO>>(transactions);
        }

        public async Task<List<TransactionDTO>> GetAllTransactionsOfUser(string username)
        {
            var transactions = await Entities
                .Include(t => t.Wallet)
                .Where(t => t.Wallet != null && t.Wallet.UserName == username)
                .ToListAsync();
            return _mapper.Map<List<TransactionDTO>>(transactions);
        }

        public async Task<bool> AddTransaction(int walletId, double amount, string description, string paymentCode)
        {
            var trans = new Transactions()
            {
                walletId = walletId,
                OrderCode = paymentCode,
                CreatedDate = DateTimeHelper.GetVietnamNow(),
                Amount = amount,
                Description = description,
                Status = "Success"
            };
            Entities.Add(trans);
            int result = await _dbContext.SaveChangesAsync();

            return result > 0;
        }
    }
}
