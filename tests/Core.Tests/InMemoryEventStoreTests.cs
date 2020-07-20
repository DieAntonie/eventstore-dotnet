using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace Core.Tests
{
    public class InMemoryEventStoreTests
    {
        internal class Event1 : EventMessage { }

        internal class Event2 : EventMessage { }

        internal static ManualResetEvent mre = new ManualResetEvent(false);

        [Fact]
        public void LoadEventsFor_EmptyEventStore_EmptyArrayList()
        {
            // Arrange
            var stubInMemoryEventStore = new InMemoryEventStore();

            // Act
            var loadedEvents = stubInMemoryEventStore.LoadEventsFor(Guid.NewGuid());

            // Assert
            Assert.Equal(new ArrayList(), loadedEvents);
        }

        [Fact]
        public void LoadEventsFor_SavedEventsWithSameGUID_ReturnsSavedEvents()
        {
            // Arrange
            var stubInMemoryEventStore = new InMemoryEventStore();
            var stubGuid = Guid.NewGuid();
            var mockEventList = new ArrayList() { new Event1(), new Event2() };
            stubInMemoryEventStore.Save(
                new AdditionalEvents(stubGuid, mockEventList));

            // Act
            var loadedEvents = stubInMemoryEventStore.LoadEventsFor(stubGuid);

            // Assert
            Assert.Equal(mockEventList, loadedEvents);
        }

        [Fact]
        public void Save_WithConcurrencyConflict_ThrowsConcurrencyConflictOccurredException()
        {
            // Arrange
            var stubInMemoryEventStore = new InMemoryEventStore();
            var stubGuid = Guid.NewGuid();
            var mockEventList = new ArrayList() { new Event1(), new Event2() };

            // Act
            Action saveConcurrencyConflict = () => stubInMemoryEventStore
                .Save(new AdditionalEvents(stubGuid, mockEventList, 2));

            // Assert
            Assert.Throws<ConcurrencyConflictOccurred>(saveConcurrencyConflict);
        }
    }
}
