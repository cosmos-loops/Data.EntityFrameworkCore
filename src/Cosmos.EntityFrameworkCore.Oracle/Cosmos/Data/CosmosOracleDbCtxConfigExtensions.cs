using System;
using Cosmos.Data.Core.Registrars;
using Cosmos.EntityFrameworkCore;
using Cosmos.EntityFrameworkCore.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cosmos.Data
{
    /// <summary>
    /// Cosmos Oracle DbContext configuration extensions.
    /// </summary>
    public static class CosmosOracleDbContextConfigExtensions
    {
        /// <summary>
        /// Use EntityFramework Core with Oracle
        /// </summary>
        /// <param name="context"></param>
        /// <param name="optAct"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IDbContextConfig UseEfCoreWithOracle<TContext>(
            this DbContextConfig context, Action<EfCoreOptions> optAct = null)
            where TContext : DbContext, IEfContext
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var opt = EfCoreOptionsHelper.CreateOptions(optAct);

            EfCoreOptionsHelper.GuardOptions(opt);

            context.RegisterDbContext(services =>
            {
                if (!string.IsNullOrWhiteSpace(opt.ConnectionString))
                {
                    EfCoreOptionsRegistrar.Register<TContext>(opt);
                    services.AddDbContext<TContext>(builder => builder.UseOracle(opt.ConnectionString));
                }
                else if (!string.IsNullOrWhiteSpace(opt.ConnectionName))
                {
                    EfCoreOptionsRegistrar.Register<TContext>(opt);
                    services.AddDbContext<TContext>((provider, builder) => builder.UseOracle(provider.GetService<IConfigurationRoot>().GetConnectionString(opt.ConnectionName)));
                }
                else
                {
                    throw new InvalidOperationException("Connection name or string cannot be empty.");
                }
            });

            return context;
        }

        /// <summary>
        /// Use EntityFramework Core with Oracle
        /// </summary>
        /// <param name="context"></param>
        /// <param name="optAct"></param>
        /// <typeparam name="TContextService"></typeparam>
        /// <typeparam name="TContextImplementation"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IDbContextConfig UseEfCoreWithOracle<TContextService, TContextImplementation>(
            this DbContextConfig context, Action<EfCoreOptions> optAct = null)
            where TContextService : IEfContext
            where TContextImplementation : DbContext, TContextService
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var opt = EfCoreOptionsHelper.CreateOptions(optAct);

            EfCoreOptionsHelper.GuardOptions(opt);

            context.RegisterDbContext(services =>
            {
                if (!string.IsNullOrWhiteSpace(opt.ConnectionString))
                {
                    EfCoreOptionsRegistrar.Register<TContextImplementation>(opt);
                    services.AddDbContext<TContextService, TContextImplementation>(
                        builder => builder.UseOracle(opt.ConnectionString));
                }
                else if (!string.IsNullOrWhiteSpace(opt.ConnectionName))
                {
                    EfCoreOptionsRegistrar.Register<TContextImplementation>(opt);
                    services.AddDbContext<TContextService, TContextImplementation>(
                        (provider, builder) => builder.UseOracle(provider.GetService<IConfigurationRoot>().GetConnectionString(opt.ConnectionName)));
                }
                else
                {
                    throw new InvalidOperationException("Connection name or string cannot be empty.");
                }
            });

            return context;
        }
    }
}