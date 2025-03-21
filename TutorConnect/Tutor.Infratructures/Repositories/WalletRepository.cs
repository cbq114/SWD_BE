using Microsoft.EntityFrameworkCore;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Configurations;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Persistence;
using Tutor.Shared.Helper;

namespace Tutor.Infratructures.Repositories
{
    public class WalletRepository : Repository<Wallet>, IWalletRepository
    {

        public WalletRepository(TutorDBContext dbContext) : base(dbContext)
        {
        }

        public async Task InitializeUserWallet(string userName)
        {
            var wallet = new Wallet
            {
                UserName = userName,
                Balance = 0,
                TransactionTime = DateTimeHelper.GetVietnamNow()
            };
            await Entities.AddAsync(wallet);
        }

        public async Task<double> GetWalletBalance(string userName)
        {
            var wallet = await Entities.FirstOrDefaultAsync(w => w.UserName == userName);
            return wallet?.Balance ?? 0;
        }

        public async Task<bool> DeductBalance(string userName, double amount)
        {
            var wallet = await Entities.FirstOrDefaultAsync(w => w.UserName == userName);
            if (wallet == null || wallet.Balance < amount)
                return false;

            wallet.Balance -= amount;
            await base.Update(wallet);
            return true;
        }

        public async Task<bool> AddMoney(string userName, double amount)
        {
            var wallet = await Entities.FirstOrDefaultAsync(w => w.UserName == userName);
            if (wallet == null)
                return false;

            wallet.Balance += amount;
            await base.Update(wallet);
            return true;
        }

        public async Task<Wallet> GetWalletByUsername(string username)
        {
            var wallet = await Entities.Include(T => T.Transactions).FirstOrDefaultAsync(w => w.UserName == username);
            if (wallet == null)
                Console.WriteLine($"Cannot found wallet with username: {username}");

            return wallet;
        }
    }
}
