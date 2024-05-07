using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using PlanIT.API.Data;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Services;
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
            // Removes DbContextOptions
            var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(DbContextOptions<PlanITDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Sets up new DbContext
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


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "TestScheme";
                options.DefaultChallengeScheme = "TestScheme";
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });



        });   

    }

    
    // Starts container
    public async Task InitializeAsync()
    {
        await _mySqlContainer.StartAsync();
    }

    // Stops container
    public new async Task DisposeAsync()
    {
        await _mySqlContainer.StopAsync();
    }

}