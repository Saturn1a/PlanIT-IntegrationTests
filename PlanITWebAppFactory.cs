using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlanIT.API.Data;
using Testcontainers.MySql;
using Microsoft.AspNetCore.Authentication;

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
            
            var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(DbContextOptions<PlanITDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            
            services.AddDbContext<PlanITDbContext>(options =>
            {
                options.UseMySql(_mySqlContainer.GetConnectionString(), new MySqlServerVersion(new Version(8, 0, 33)), builder =>
                {
                    builder.EnableRetryOnFailure();
                });
            });

            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "TestScheme";
                options.DefaultChallengeScheme = "TestScheme";
            }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });

            
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
