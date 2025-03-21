using Microsoft.EntityFrameworkCore.Storage;

namespace Tutor.Infratructures.Configurations
{
    public interface IUnitOfWork : IDisposable
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task<int> SaveChangesAsync();
    }
}
