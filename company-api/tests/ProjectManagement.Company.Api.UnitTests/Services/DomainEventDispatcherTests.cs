using MediatR;
using Moq;
using ProjectManagement.CompanyAPI.Contracts;
using ProjectManagement.CompanyAPI.Services;

namespace ProjectManagement.CompanyAPI.UnitTests.Services;

[ExcludeFromCodeCoverage]
public class DomainEventDispatcherTests
{
    private readonly Mock<IMediator> _mockMediator = new ();
    private readonly DomainEventDispatcher _sut;

    public DomainEventDispatcherTests()
    {
        _sut = new DomainEventDispatcher(_mockMediator.Object);
    }

    [Fact]
    public async Task DispatchAndClearEvents_ShouldDispatchAllEventsForEachEntity()
    {
        // Arrange
        TestEntityBase entityWithEvents = new TestEntityBase();
        entityWithEvents.AddEvent();

        TestEntityBase[] entities =
        {
            new TestEntityBase(),
            new TestEntityBase(),
            new TestEntityBase(),
            entityWithEvents,
        };

        DomainEventBase[] expectedEvents = entities.SelectMany(x => x.DomainEvents).ToArray();

        // Act
        await _sut.DispatchAndClearEvents(entities);

        // Assert
        foreach (DomainEventBase expectedEvent in expectedEvents)
        {
            _mockMediator.Verify(x => x.Publish(expectedEvent, It.IsAny<CancellationToken>()), Times.Once());
        }
    }

    [Fact]
    public async Task DispatchAndClearEvents_ShouldClearAllDomainEventsForEachEntity()
    {
        // Arrange
        TestEntityBase[] entities =
        {
            new TestEntityBase(),
            new TestEntityBase(),
            new TestEntityBase(),
        };

        // Act
        await _sut.DispatchAndClearEvents(entities);

        // Assert
        foreach (EntityBase entity in entities)
        {
            Assert.Empty(entity.DomainEvents);
        }
    }

    private class TestEntityBase : EntityBase
    {
        public void AddEvent()
        {
            RegisterDomainEvent(new TestDomainEvent());
        }
    }

    private class TestDomainEvent : DomainEventBase
    {
    }
}