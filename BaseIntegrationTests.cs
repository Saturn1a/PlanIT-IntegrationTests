using Microsoft.Extensions.DependencyInjection;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Services;
using PlanIT.API.Services.Interfaces;


namespace PlanITAPI.IntegrationTests.Docker;

// Base clase
public class BaseIntegrationTests : IClassFixture<PlanITWebAppFactory>, IDisposable
{
    private readonly IServiceScope _serviceScope;

    public BaseIntegrationTests(PlanITWebAppFactory factory)
    {
        _serviceScope = factory.Services.CreateScope();
        Client = factory.CreateClient();

       
        UserService = _serviceScope.ServiceProvider.GetService<IUserService>();
        // EventService = _serviceScope.ServiceProvider.GetService<IService<Event>>();
        
        
    }



    public HttpClient Client { get; init; }
    public IUserService? UserService { get; init; }
    
    // public IService<Event>? EventService { get; init; }
   


    public void Dispose()
    {
        Client?.Dispose();
    }
}