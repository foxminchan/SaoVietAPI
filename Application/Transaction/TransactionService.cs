using Infrastructure;

namespace Application.Transaction
{
    public class TransactionService
    {
        private readonly ApplicationDbContext _context;

        public TransactionService(ApplicationDbContext context) => _context = context;

        public void ExecuteTransaction(Action action)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                action();
                _context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public T ExecuteTransaction<T>(Func<T> action)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var result = action();
                _context.SaveChanges();
                transaction.Commit();
                return result;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
