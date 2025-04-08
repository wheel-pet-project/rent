using JetBrains.Annotations;

namespace IntegrationTests.UnitOfWork;

[TestSubject(typeof(Infrastructure.Adapters.Postgres.UnitOfWork))]
public class UnitOfWorkShould : IntegrationTestBase
{
    // private readonly JsonSerializerSettings _jsonSerializerSettings = new()
    // {
    //     ContractResolver = new PrivateSetterContractResolver(),
    //     TypeNameHandling = TypeNameHandling.All
    // };
    //
    // private readonly Booking _booking = Booking.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    //
    // [Fact]
    // public async Task SaveDomainEventToOutbox()
    // {
    //     // Arrange
    //     await AddBooking(_booking);
    //
    //     var vehicleCheck = Check.Create(_booking, TimeProvider.System);
    //
    //     vehicleCheck.Complete(TimeProvider.System);
    //     var expectedDomainEvent = vehicleCheck.DomainEvents[0];
    //     var uowBuilder = new UnitOfWorkBuilder();
    //     var uow = uowBuilder.Build(Context);
    //
    //     // Act
    //     await Context.AddAsync(vehicleCheck, TestContext.Current.CancellationToken);
    //     await uow.Commit();
    //
    //     // Assert
    //     Context.ChangeTracker.Clear();
    //     var outboxEvent = Context.Outbox.FirstOrDefault();
    //     var eventParsedContent =
    //         JsonConvert.DeserializeObject<DomainEvent>(outboxEvent!.Content, _jsonSerializerSettings);
    //     Assert.NotNull(eventParsedContent);
    //     Assert.Equivalent(expectedDomainEvent, eventParsedContent);
    // }
    //
    // private async Task AddBooking(Booking booking)
    // {
    //     await Context.Bookings.AddAsync(booking);
    //     await Context.SaveChangesAsync();
    // }
    //
    // private class UnitOfWorkBuilder
    // {
    //     public Infrastructure.Adapters.Postgres.UnitOfWork Build(DataContext context)
    //     {
    //         return new Infrastructure.Adapters.Postgres.UnitOfWork(context);
    //     }
    // }
}