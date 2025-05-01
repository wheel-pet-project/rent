using Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;
using JetBrains.Annotations;
using JsonNet.ContractResolvers;
using Newtonsoft.Json;
using Xunit;

namespace IntegrationTests.Inbox;

[TestSubject(typeof(Infrastructure.Adapters.Postgres.Inbox.Inbox))]
public class InboxShould : IntegrationTestBase
{
    private Infrastructure.Adapters.Postgres.Inbox.Inbox _inbox = null!;
    
    private readonly IConvertibleToCommand _event =
        new VehicleAddedConsumerEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    
    private readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        ContractResolver = new PrivateSetterContractResolver()
    };
    
    [Fact]
    public async Task SaveEvent()
    {
        // Arrange
        _inbox = new Infrastructure.Adapters.Postgres.Inbox.Inbox(DataSource);
    
        // Act
        await _inbox.Save(_event);
    
        // Assert
        var existsEvents = Context.Inbox.Take(1).ToList();
        var actualEvent =
            JsonConvert.DeserializeObject<IConvertibleToCommand>(existsEvents.First().Content, _jsonSerializerSettings);
        Assert.NotNull(actualEvent);
        Assert.Equivalent(_event, actualEvent);
    }
}