using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using PlanIT.API.Models.DTOs;
using System.Net;

namespace PlanITAPI.IntegrationTests.Docker.Controllers;

public class EventControllerTests : IAsyncLifetime
{
    private readonly PlanITWebAppFactory _factory;
    private HttpClient? _client;
    public EventControllerTests()
    {
        _factory = new PlanITWebAppFactory();
    }

    public async Task InitializeAsync()
    {
        await _factory.InitializeAsync();
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task GetEventById_ReturnResults()
    {
        // Arrange
        var expectedEventName = "Dinner Date";
        var expectedEventId = 2; 
        var expectedUserId = 1;
       
  
        // Act
        var response = await _client.GetAsync($"/api/v1/Events/{expectedEventId}");


        response.EnsureSuccessStatusCode(); 
        var responseString = await response.Content.ReadAsStringAsync();
        var eventResponse = JsonConvert.DeserializeObject<EventDTO>(responseString); 

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(eventResponse);
        Assert.Equal(expectedEventName, eventResponse.Name);
        Assert.Equal(expectedEventId, eventResponse.Id);
        Assert.Equal(expectedUserId, eventResponse.UserId);

    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
        _client?.Dispose();
    }
}
