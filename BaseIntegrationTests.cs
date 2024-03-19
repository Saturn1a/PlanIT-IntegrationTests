using Microsoft.Extensions.DependencyInjection;
using PlanIT.API.Services.Interfaces;


namespace PlanITAPI.IntegrationTests.Docker;

// Fixture-klasse (Base-testklasse som andre tester kan arve fra)
public class BaseIntegrationTests : IClassFixture<PlanITWebAppFactory>, IDisposable
{
    private readonly IServiceScope _serviceScope;

    public BaseIntegrationTests(PlanITWebAppFactory factory)
    {
        _serviceScope = factory.Services.CreateScope();
        Client = factory.CreateClient();

        // Tar inn for å teste mot service
        UserService = _serviceScope.ServiceProvider.GetService<IUserService>();
    }

    public HttpClient Client { get; init; }

    public IUserService? UserService { get; init; }

    public void Dispose()
    {
        Client?.Dispose();
    }
}