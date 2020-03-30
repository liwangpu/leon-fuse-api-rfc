using Base.Domain.Common;
using Base.Infrastructure;
using IDS.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace IDS.Infrastructure
{
    public class IDSAppContext : DbContext, IUnitOfWork
    {
        #region propeties
        private readonly IMediator _mediator;
        private IDbContextTransaction _currentTransaction;
        public const string DEFAULT_SCHEMA = "idsapp";
        public IDbContextTransaction GetCurrentTransaction => _currentTransaction;
        public bool HasActiveTransaction => _currentTransaction != null;
        #endregion

        #region ctor
        protected IDSAppContext()
        {
        }

        public IDSAppContext(DbContextOptions<IDSAppContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        #endregion

        #region OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DEFAULT_SCHEMA);
            modelBuilder.ApplyConfiguration(new IdentityEntityConfiguration());
            modelBuilder.ApplyConfiguration(new IdentityGrantEntityConfiguration());
            modelBuilder.ApplyConfiguration(new OrganizationEntityConfiguration());
        }
        #endregion

        #region SaveEntitiesAsync
        /// <summary>
        /// 默认是SaveChangesAsync后发布域事件
        /// 如果需要自己控制发布前后,使用同名重载函数
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SaveEntitiesAsync(true, cancellationToken);
        }
        #endregion

        #region SaveEntitiesAsync
        public async Task<bool> SaveEntitiesAsync(bool publishDomainEventAfterSaveChangeAsync, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
            //await _mediator.DispatchDomainEventsAsync(this);

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed through the DbContext will be committed
            //var result = await base.SaveChangesAsync();

            if (publishDomainEventAfterSaveChangeAsync)
            {
                await base.SaveChangesAsync();
                await _mediator.DispatchDomainEventsAsync(this);
            }
            else
            {
                await _mediator.DispatchDomainEventsAsync(this);
                await base.SaveChangesAsync();
            }
            return true;
        }
        #endregion

        #region BeginTransactionAsync
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }
        #endregion

        #region CommitTransactionAsync
        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
        #endregion

        #region RollbackTransaction
        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
        #endregion
    }
}
