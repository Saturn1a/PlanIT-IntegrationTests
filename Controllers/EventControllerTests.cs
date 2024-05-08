using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using PlanIT.API.Models.DTOs;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace PlanITAPI.IntegrationTests.Docker.Controllers;

public class EventControllerTests : BaseIntegrationTests
{
    public EventControllerTests(PlanITWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task GetEventById_ReturnResults()
    {
        // Arrange
        var expectedEventName = "Dinner Date";
        var expectedEventId = 2;
        var expectedUserId = 1;

        // Act
        var response = await Client.GetAsync($"/api/v1/Events/{expectedEventId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var eventResponse = JsonConvert.DeserializeObject<EventDTO>(responseString);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(eventResponse);
        Assert.Equal(expectedEventName, eventResponse.Name);
        Assert.Equal(expectedEventId, eventResponse.Id);
        Assert.Equal(expectedUserId, eventResponse.UserId);
    }
}
