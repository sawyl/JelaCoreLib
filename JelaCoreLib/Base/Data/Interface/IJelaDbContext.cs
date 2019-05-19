using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JelaCoreLib.Base.Data.Interface
{
    /// <summary>
    /// Interface to provide access to functions of DbContext and IdentityDbContext allowing better reuse of the code.
    /// </summary>
    public interface IJelaDbContext
    {
        #region properties

        DatabaseFacade Database { get; }
        ChangeTracker ChangeTracker { get; }
        IModel Model { get; }

        #endregion

        #region methods

        EntityEntry<TEntity> Add<TEntity>(TEntity entity) where TEntity : class;
        EntityEntry Add(object entity);
        Task<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class;
        Task<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default(CancellationToken));

        void AddRange(params object[] entities);
        void AddRange(IEnumerable<object> entities);
        Task AddRangeAsync(params object[] entities);
        Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default(CancellationToken));

        EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class;
        EntityEntry Attach(object entity);
        void AttachRange(params object[] entities);
        void AttachRange(IEnumerable<object> entities);

        void Dispose();

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        EntityEntry Entry(object entity);

        object Find(Type entityType, params object[] keyValues);
        TEntity Find<TEntity>(params object[] keyValues) where TEntity : class;
        Task<object> FindAsync(Type entityType, params object[] keyValues);
        Task<object> FindAsync(Type entityType, object[] keyValues, CancellationToken cancellationToken);
        Task<TEntity> FindAsync<TEntity>(params object[] keyValues) where TEntity : class;
        Task<TEntity> FindAsync<TEntity>(object[] keyValues, CancellationToken cancellationToken) where TEntity : class;

        /*
        void OnConfiguring(DbContextOptionsBuilder optionsBuilder);
        void OnModelCreating(ModelBuilder modelBuilder);
        */

        EntityEntry Remove(object entity);
        void RemoveRange(params object[] entities);
        void RemoveRange(IEnumerable<object> entities);

        int SaveChanges();
        int SaveChanges(bool acceptAllChangesOnSuccess);
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        EntityEntry Update(object entity);
        void UpdateRange(params object[] entities);
        void UpdateRange(IEnumerable<object> entities);

        #endregion
    }
}
