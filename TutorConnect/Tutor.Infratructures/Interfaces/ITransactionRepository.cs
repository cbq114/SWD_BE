using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Infratructures.Models.PaymentModel;

namespace Tutor.Infratructures.Interfaces
{
    public interface ITransactionRepository
    {
        Task<List<TransactionDTO>> GetAllTransactions();
        Task<List<TransactionDTO>> GetAllTransactionsOfUser(string username);
        Task<bool> AddTransaction(int walletId, double amount, string description, string paymentCode);
    }
}
