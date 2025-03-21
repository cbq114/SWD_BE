using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Infratructures.Models.PaymentModel;

namespace Tutor.Applications.Interfaces
{
    public interface ITransactionService
    {
        Task<List<TransactionDTO>> GetAllTransactions();
        Task<List<TransactionDTO>> GetAllTransactionsOfUser(string username);
    }
}
