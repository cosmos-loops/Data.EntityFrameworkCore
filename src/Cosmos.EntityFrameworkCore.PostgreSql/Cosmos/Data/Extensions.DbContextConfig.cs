using System;
using Cosmos.Data.Context;
using Cosmos.EntityFrameworkCore;
using Cosmos.EntityFrameworkCore.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IEfCoreDbContext = Cosmos.EntityFrameworkCore.IDbContext;

namespace Cosmos.Data
{
    /// <summary>
    /// Extensions for Cosmos DbContext
    /// </summary>
    public static class DbContextConfigExtensions
    {
        /// <summary>
        /// Use EntityFramework Core with PostgreSql
        /// </summary>
        /// <param name="config"></param>
        /// <param name="optAct"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IDbContextConfig UseEfCoreWithPostgreSql<TContext>(
            this DbContextConfig config, Action<EfCoreOptions> optAct = null)
            where TContext : DbContext, IEfCoreDbContext
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var options = InternalEfCoreRegistrar.GetOptions(optAct);

            return CoreRegistrar.Register(config, options,
                s => s.AddDbContext<TContext>(o => o.UseNpgsql(options.ConnectionString).UseLolita()));
        }

        /// <summary>
        /// Use EntityFramework Core with PostgreSql
        /// </summary>
        /// <param name="config"></param>
        /// <param name="optAct"></param>
        /// <typeparam name="TCtxtService"></typeparam>
        /// <typeparam name="TCtxImplementation"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IDbContextConfig UseEfCoreWithPostgreSql<TCtxtService, TCtxImplementation>(
            this DbContextConfig config, Action<EfCoreOptions> optAct = null)
            where TCtxtService : IEfCoreDbContext
            where TCtxImplementation : DbContext, TCtxtService
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var options = InternalEfCoreRegistrar.GetOptions(optAct);

            return CoreRegistrar.Register(config, options,
                s => s.AddDbContext<TCtxtService, TCtxImplementation>(o => o.UseNpgsql(options.ConnectionString).UseLolita()));
        }

        private static class CoreRegistrar
        {
            public static DbContextConfig Register(DbContextConfig context, EfCoreOptions options, Action<IServiceCollection> action)
            {
                InternalEfCoreRegistrar.GuardEfCoreOptions(options);
                context.RegisterDbContext(services => { action?.Invoke(services); });
                return context;
            }
        }
    }
}