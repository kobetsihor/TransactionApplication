using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TransactionApplication.BL.Abstractions;
using TransactionApplication.BL.Parsers;
using TransactionApplication.BL.Services;
using TransactionApplication.BL.Validators;
using TransactionApplication.DAL.Context;
using TransactionApplication.DAL.Repositories;

namespace TransactionApplication.BL.Configuration
{
    public static class DIBinder
    {
        public static void ConfigureDependencies(this IServiceCollection services)
        {
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddTransient<ITransactionService, TransactionService>();
            services.AddTransient<ITransactionParserProvider, TransactionParserProvider>();
            services.AddTransient<ITransactionDataParser, CsvTransactionParser>();
            services.AddTransient<ITransactionDataParser, XmlTransactionParser>();
            services.AddTransient<ParseResultsValidator>();
        }

        public static void ConfigureDatabase(this IServiceCollection services)
        {
            string connection = "Server=DESKTOP-AAAARFJ;Database=TransactionsDb;Trusted_Connection=True;";
            services.AddDbContext<ITransactionContext, TransactionContext>(options => options.UseSqlServer(connection));
        }
    }
}