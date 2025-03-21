using Tutor.Domains.Entities;

namespace Tutor.Infratructures.Interfaces
{
    public interface IWalletRepository
    {
        Task InitializeUserWallet(string userName);
        Task<double> GetWalletBalance(string userName);
        Task<bool> DeductBalance(string userName, double amount);
        Task<bool> AddMoney(string userName, double amount);
        Task<Wallet> GetWalletByUsername(string username);
    }
}
