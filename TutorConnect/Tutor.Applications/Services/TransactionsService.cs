using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Applications.Interfaces;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.PaymentModel;

namespace Tutor.Applications.Services
{
    public class TransactionsService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        public TransactionsService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<List<TransactionDTO>> GetAllTransactions()
        {
            return await _transactionRepository.GetAllTransactions();
        }

        public Task<List<TransactionDTO>> GetAllTransactionsOfUser(string username)
        {
            return _transactionRepository.GetAllTransactionsOfUser(username);
        }
    }
}
