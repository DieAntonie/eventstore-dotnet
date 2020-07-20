using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Core.Tests
{
    public class AggregateTests
    {
        private class AppliedEvent1 : EventMessage { }

        private class AppliedEvent2 : EventMessage { }

        private class UnappliedEvent : EventMessage { }

        private class FakeAggregate : BaseAggregate,
            Applies<AppliedEvent1>,
            Applies<AppliedEvent2>
        {
            public List<object> applied_events;

            public FakeAggregate()
            {
                applied_events = new List<object>();
            }

            public void Apply(AppliedEvent1 e) => applied_events.Add(e);

            public void Apply(AppliedEvent2 e) => applied_events.Add(e);
        }

        [Fact]
        public void ApplyEvent_WithSupportedEvent_AppliesEvent()
        {
            // Arrange
            var mockAggregate = new FakeAggregate();
            var mockEvent = new AppliedEvent1();

            // Act
            mockAggregate.ApplyEvent(mockEvent);

            // Assert
            Assert.Equal(mockEvent, mockAggregate.applied_events[0]);
        }

        [Fact]
        public void ApplyEvent_WithUnsupportedEvent_ThrowsUnapplicableEventException()
        {
            // Arrange
            var mockAggregate = new FakeAggregate();
            var stubEvent = new UnappliedEvent();

            // Act
            Action applyUnapplicable = () => mockAggregate.ApplyEvent(stubEvent);

            // Assert
            Assert.Throws<UnapplicableEvent>(applyUnapplicable);
        }

        [Fact]
        public void ApplyEvents_WithSupportedEvents_AppliesEvents()
        {
            // Arrange
            var mockAggregate = new FakeAggregate();
            var mockEvents = new List<object>
            {
                new AppliedEvent1(),
                new AppliedEvent2()
            };

            // Act
            mockAggregate.ApplyEvents(mockEvents);

            // Assert
            Assert.Equal(mockEvents, mockAggregate.applied_events);
        }

        [Fact]
        public void ApplyEvents_WithUnsupportedEvents_ThrowsUnapplicableEventException()
        {
            // Arrange
            var mockAggregate = new FakeAggregate();
            var stubEvents = new List<object>
            {
                new AppliedEvent1(),
                new UnappliedEvent()
            };

            // Act
            Action applyUnapplicable = () => mockAggregate.ApplyEvents(stubEvents);

            // Assert
            Assert.Throws<UnapplicableEvent>(applyUnapplicable);
        }
    }
}
