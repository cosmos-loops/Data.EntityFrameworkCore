using System;
using Cosmos.Data.Core.Registrars;
using Cosmos.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cosmos.Data
{
    /// <summary>
    /// Extensions for Cosmos DbContext
    /// </summary>
    public static class DbContextConfigExtensions
    {
        /// <summary>
        /// Use EntityFramework Core with MySql
        /// </summary>
        /// <param name="context"></param>
        /// <param name="optAct"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IDbContextConfig UseEfCoreWithMySql<TContext>(this DbContextConfig context, Action<EfCoreOptions> optAct = null)
            where TContext : DbContext, IEfContext
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var opt = new EfCoreOptions();
            optAct?.Invoke(opt);

            context.RegisterDbContext(services =>
            {
                if (!string.IsNullOrWhiteSpace(opt.ConnectionString))
                {
                    services.AddDbContext<TContext>(o => o.UseMySql(opt.ConnectionString));
                }
                else if (!string.IsNullOrWhiteSpace(opt.ConnectionName))
                {
                    services.AddDbContext<TContext>((p, o) =>
                        o.UseMySql(p.GetService<IConfigurationRoot>().GetConnectionString(opt.ConnectionName)));
                }
                else
                {
                    throw new InvalidOperationException("Connection name or string cannot be empty.");
                }
            });

            return context;
        }

        /// <summary>
        /// Use EntityFramework Core with MySql
        /// </summary>
        /// <param name="context"></param>
        /// <param name="optAct"></param>
        /// <typeparam name="TContextService"></typeparam>
        /// <typeparam name="TContextImplementation"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IDbContextConfig UseEfCoreWithMySql<TContextService, TContextImplementation>(
            this DbContextConfig context, Action<EfCoreOptions> optAct = null)
            where TContextService : IEfContext
            where TContextImplementation : DbContext, TContextService
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var opt = new EfCoreOptions();
            optAct?.Invoke(opt);

            context.RegisterDbContext(services =>
            {
                if (!string.IsNullOrWhiteSpace(opt.ConnectionString))
                {
                    services.AddDbContext<TContextService, TContextImplementation>(o => o.UseMySql(opt.ConnectionString));
                }
                else if (!string.IsNullOrWhiteSpace(opt.ConnectionName))
                {
                    services.AddDbContext<TContextService, TContextImplementation>((p, o) =>
                        o.UseMySql(p.GetService<IConfigurationRoot>().GetConnectionString(opt.ConnectionName)));
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