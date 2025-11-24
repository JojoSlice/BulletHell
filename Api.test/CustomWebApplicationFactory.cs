using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repository.Data;
using System.ComponentModel;

namespace Api.test;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // remove real services
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<MyDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var dbConnectionDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(SqliteConnection));
            if (dbConnectionDescriptor != null)
            {
                services.Remove(dbConnectionDescriptor);
            }

            // setup services in memory
            services.AddSingleton<SqliteConnection>(Container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                return connection;
            });

            services.AddDbContext<MyDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<SqliteConnection>();
                options.UseSqlite(connection);
            });
        });
    }

    // Create host
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
        db.Database.EnsureCreated();

        return host;
    }
}