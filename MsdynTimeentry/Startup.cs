using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MsdynTimeentry.DataStorage;
using MsdynTimeentry.DataStorage.Dataverse;
using System;

[assembly: FunctionsStartup(typeof(MsdynTimeentry.Startup))]
namespace MsdynTimeentry
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            builder.Services.AddSingleton<IDataStorageFactory>((s) => new DataStorageFactory(connectionString));
        }
    }
}
