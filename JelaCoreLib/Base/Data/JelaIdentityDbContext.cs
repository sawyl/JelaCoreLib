using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JelaCoreLib.Base.Data.Interface;
using JelaCoreLib.Base.Model.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JelaCoreLib.Base.Data
{
    /// <summary>
    /// Base for IdentityDbContext, in the special way by Jela.
    /// </summary>
    public abstract class JelaIdentityDbContext<
        TUser,
        TRole,
        TKey
    > : JelaIdentityDbContext<
        TUser,
        TRole,
        TKey,
        IdentityUserClaim<TKey>,
        IdentityUserRole<TKey>,
        IdentityUserLogin<TKey>,
        IdentityRoleClaim<TKey>,
        IdentityUserToken<TKey>
    >
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Create new instance of the context.
        /// </summary>
        /// <param name="options">Options for the dbcontext.</param>
        public JelaIdentityDbContext(/*HttpContextAccessor accessor,*/ DbContextOptions options)
            : base(options)
        {
            //_accessor = accessor;
        }
    }

    /// <summary>
    /// Base for IdentityDbContext, in the special way by Jela.I hear 
    /// </summary>
    /// <typeparam name="TUser">The type of user objects.</typeparam>
    /// <typeparam name="TRole">The type of role objects.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for users and roles.</typeparam>
    /// <typeparam name="TUserClaim">The type of the user claim object.</typeparam>
    /// <typeparam name="TUserRole">The type of the user role object.</typeparam>
    /// <typeparam name="TUserLogin">The type of the user login object.</typeparam>
    /// <typeparam name="TRoleClaim">The type of the role claim object.</typeparam>
    /// <typeparam name="TUserToken">The type of the user token object.</typeparam>
    public abstract class JelaIdentityDbContext<
        TUser,
        TRole,
        TKey,
        TUserClaim,
        TUserRole,
        TUserLogin,
        TRoleClaim,
        TUserToken
    > : IdentityDbContext<
        TUser,
        TRole,
        TKey,
        TUserClaim,
        TUserRole,
        TUserLogin,
        TRoleClaim,
        TUserToken
    >, IJelaDbContext
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
    {
        /// <summary>
        /// Name of the property for deletion.
        /// </summary>
        private const string _isDeletedProperty = "IsDeleted";
        /// <summary>
        /// Metadata for the property method.
        /// </summary>
        private static readonly MethodInfo _deletedPropertyMethod = typeof(EF).GetMethod(nameof(EF.Property), BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(typeof(bool));

        /// <summary>
        /// Name of the property for deletion.
        /// </summary>
        private const string _communitySpecificProperty = "CommunityID";
        /// <summary>
        /// Metadata for the property method.
        /// </summary>
        private static readonly MethodInfo _communitySpecificPropertyMethod = typeof(EF).GetMethod(nameof(EF.Property), BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(typeof(int));

        /// <summary>
        /// Handle for the accessor for user details.
        /// </summary>
        //protected readonly HttpContextAccessor _accessor;


        /// <summary>
        /// Create new instance of the context.
        /// </summary>
        /// <param name="options">Options for the dbcontext.</param>
        public JelaIdentityDbContext(/*HttpContextAccessor accessor,*/ DbContextOptions options)
            : base(options)
        {
            //_accessor = accessor;
        }

        /// <summary>
        /// Configure the model that was discovered by convention from the entity types exposed in DbSet<TEntity> properties on your derived context. 
        /// The resulting model may be cached and re-used for subsequent instances of your derived context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Go through the entities to add the custom elements.
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletable).IsAssignableFrom(entity.ClrType) == true)
                {
                    entity
                        .AddProperty(_isDeletedProperty, typeof(bool));

                    //Default the value of IsDeleted to false.
                    modelBuilder
                       .Entity(entity.ClrType)
                       .Property(_isDeletedProperty).HasDefaultValue(false);
                    //Add index for performance.
                    modelBuilder
                        .Entity(entity.ClrType)
                        .HasIndex(_isDeletedProperty).HasName($"IX_{_isDeletedProperty}");
                    //And apply filter for queries.
                    modelBuilder
                        .Entity(entity.ClrType)
                        .HasQueryFilter(GetIsDeletedRestriction(entity.ClrType));
                }

                if (typeof(ICommunitySpecific).IsAssignableFrom(entity.ClrType) == true)
                {
                    entity
                        .AddProperty(_communitySpecificProperty, typeof(int));

                    //Default the value to user's communityID.
                    modelBuilder
                       .Entity(entity.ClrType)
                       .Property(_communitySpecificProperty).HasDefaultValue(0/*_accessor.HttpContext.User.Identity*/);

                    //Add index for performance.
                    modelBuilder
                        .Entity(entity.ClrType)
                        .HasIndex(_communitySpecificProperty).HasName($"IX_{_communitySpecificProperty}");
                    //And apply filter for queries.
                    modelBuilder
                        .Entity(entity.ClrType)
                        .HasQueryFilter(GetBelongsToCommunityRestriction(entity.ClrType));
                }
            }

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public override int SaveChanges()
        {
            OnBeforeSaving();
            return base.SaveChanges();
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">Indicates whether AcceptAllChanges() is called after the changes have been sent successfully to the database.</param>
        /// <returns>The number of state entries written to the database.</returns>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the database.
        /// </summary>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>The number of state entries written to the database.</returns>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the database.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">Indicates whether AcceptAllChanges() is called after the changes have been sent successfully to the database.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>The number of state entries written to the database.</returns>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// Custom functionality that will be called before saving the changes to the database.
        /// </summary>
        private void OnBeforeSaving()
        {
            //Loop through all ISoftDeletables and change the behaviour when deleted to perform the soft delete.
            foreach (var entry in this.ChangeTracker.Entries<ISoftDeletable>().Where(e => e.State == EntityState.Deleted))
            {
                entry.Property(_isDeletedProperty).CurrentValue = true;
                entry.State = EntityState.Modified;
            }
        }

        /// <summary>
        /// Creates the lambda expression for checking the delete restriction for soft delete.
        /// </summary>
        /// <param name="type">Type for what is being checked.</param>
        /// <returns>The lambda expression for checking is deleted.</returns>
        private static LambdaExpression GetIsDeletedRestriction(Type type)
        {
            //Initialize the parameter item for lambda
            var param = Expression.Parameter(type, "item");
            //Initialize the property as a call for condition check and check it.
            var propertyCall = Expression.Call(_deletedPropertyMethod, param, Expression.Constant(_isDeletedProperty));
            var condition = Expression.MakeBinary(ExpressionType.Equal, propertyCall, Expression.Constant(false));
            //And turn into lambda.
            var lambda = Expression.Lambda(condition, param);
            return lambda;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">Type for what is being checked.</param>
        /// <returns>The lambda expression for beloning to the community.</returns>
        private static LambdaExpression GetBelongsToCommunityRestriction(Type type)
        {
            //Initialize the parameter item for lambda
            var param = Expression.Parameter(type, "item");
            //Initialize the property as a call for condition check and check it.
            var propertyCall = Expression.Call(_communitySpecificPropertyMethod, param, Expression.Constant(_communitySpecificProperty));
            var condition = Expression.MakeBinary(ExpressionType.Equal, propertyCall, Expression.Constant(-1));
            //And turn into lambda.
            var lambda = Expression.Lambda(condition, param);
            return lambda;
        }
    }
}
