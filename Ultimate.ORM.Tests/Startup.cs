using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using Xunit.DependencyInjection;

[assembly: TestFramework("Ultimate.ORM.Tests.Startup", "Ultimate.ORM.Tests")]

namespace Ultimate.ORM.Tests
{
    public class Startup : DependencyInjectionTestFramework
    {
        public Startup(IMessageSink messageSink) : base(messageSink)
        {
        }

        private string GetConnectionString(IServiceProvider sp) =>
            sp.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");

        protected void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(p =>
            {
                var connection = new SQLiteConnection(GetConnectionString(p));
                var initializeSql = File.ReadAllText("CreateData.sql");
                using var cmd = new SQLiteCommand(initializeSql, connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                return connection;
            });
        }

        protected override IHostBuilder CreateHostBuilder(AssemblyName assemblyName) =>
            base.CreateHostBuilder(assemblyName)
            .ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appSettings.json", optional: false);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices(ConfigureServices);
    }
}
