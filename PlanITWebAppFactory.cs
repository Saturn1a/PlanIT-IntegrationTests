using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using PlanIT.API.Data;
using Testcontainers.MySql;

namespace PlanITAPI.IntegrationTests.Docker;

public class PlanITWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{

    private readonly MySqlContainer _mySqlContainer;

    public PlanITWebAppFactory()
    {
        _mySqlContainer = new MySqlBuilder()
            .WithImage("tinamao/planit-db")
            .WithDatabase("planit_db")
            .WithUsername("planit-user")
            .WithPassword("5ecret-plan1t")
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // først ta bort DbContextOptions
            var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(DbContextOptions<PlanITDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            // Setter opp DbContext på nytt
            services.AddDbContext<PlanITDbContext>(options =>
            {
                options.UseMySql(
                    _mySqlContainer.GetConnectionString(),
                    new MySqlServerVersion(new Version(8, 0, 33)),
                    builder =>
                    {
                        builder.EnableRetryOnFailure();
                    });
            });
        });
    }

    // Innebygd interface
    // STARTER CONTAINER
    public async Task InitializeAsync()
    {
        await _mySqlContainer.StartAsync();
    }

    // STOPPER CONTAINER
    public new async Task DisposeAsync()
    {
        await _mySqlContainer.StopAsync();
    }

}