using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PlanIT.API.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MySql;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;

namespace PlanITAPI.IntegrationTests.Docker;

public class PlanITWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MySqlContainer _mySqlContainer;

    public PlanITWebAppFactory()
    {
        _mySqlContainer = new MySqlBuilder()
            .WithImage("hannapersson/planit-db")
            .WithDatabase("planit_db")
            .WithUsername("planit-user")
            .WithPassword("5ecret-plan1t")
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Enhanced DbContext setup for transaction management
            services.AddScoped<PlanITDbContext>(provider =>
            {
                var options = new DbContextOptionsBuilder<PlanITDbContext>()
                    .UseMySql(_mySqlContainer.GetConnectionString(), new MySqlServerVersion(new Version(8, 0, 33)))
                    .Options;
                return new PlanITDbContext(options);
            });

            // Authentication and authorization setup
            services.AddAuthentication("TestScheme").AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Bearer", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AuthenticationSchemes = new List<string> { "TestScheme" };
                });
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _mySqlContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _mySqlContainer.StopAsync();
    }
}
