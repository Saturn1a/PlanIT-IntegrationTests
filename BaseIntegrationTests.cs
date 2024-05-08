using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using PlanIT.API.Data;

namespace PlanITAPI.IntegrationTests.Docker;

public class BaseIntegrationTests : IDisposable
{
    private readonly IServiceScope _serviceScope;
    private readonly PlanITDbContext _dbContext;

    public BaseIntegrationTests(PlanITWebAppFactory factory)
    {
        _serviceScope = factory.Services.CreateScope();
        Client = factory.CreateClient();

        // Retrieve DbContext from service scope
        _dbContext = _serviceScope.ServiceProvider.GetRequiredService<PlanITDbContext>();

        // Start transaction
        _dbContext.Database.BeginTransaction();
    }

    public HttpClient Client { get; }

    public void Dispose()
    {
        // Rollback transaction after each test to cleanup the DB state
        _dbContext.Database.RollbackTransaction();
        _dbContext.Dispose();
        Client?.Dispose();
        _serviceScope.Dispose();
    }
}
