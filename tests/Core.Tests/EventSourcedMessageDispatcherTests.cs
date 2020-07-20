using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Core.Tests
{
    public class EventSourcedMessageDispatcherTests
    {
        private class FakeCommand : CommandMessage { }

        private class FakeEvent : EventMessage { }

        private class FakeEventStore : EventStore
        {
            ArrayList stored_events;

            public ICollection LoadEventsFor(Guid aggregateId)
            {
                return stored_events ?? new ArrayList();
            }

            public void Save(AdditionalEvents additionalEvents)
            {
                stored_events = additionalEvents.Events;
            }
        }

        private class HandlesFakeCommand : BaseAggregate,
            Handles<FakeCommand>
        {
            public static FakeCommand handled_command;

            public IEnumerable Handle(FakeCommand c)
            {
                handled_command = c;
                yield break;
            }
        }

        private class FirstFakeCommandHandler : BaseAggregate,
            Handles<FakeCommand>
        {

            public IEnumerable Handle(FakeCommand c)
            {
                throw new NotImplementedException();
            }
        }

        private class SecondFakeCommandHandler : BaseAggregate,
            Handles<FakeCommand>
        {
            public IEnumerable Handle(FakeCommand c)
            {
                throw new NotImplementedException();
            }
        }

        private class AppliesFakeEventHandlesFakeCommand : BaseAggregate,
            Handles<FakeCommand>, Applies<FakeEvent>
        {
            public static FakeCommand handled_command;

            public static FakeEvent applied_event;

            public void Apply(FakeEvent @event)
            {
                applied_event = @event;
            }

            public IEnumerable Handle(FakeCommand command)
            {
                handled_command = applied_event != null ? command : null;
                yield break;
            }
        }

        private class HandlesFakeCommandEmitsFakeEvents : BaseAggregate,
            Handles<FakeCommand>
        {
            public IEnumerable Handle(FakeCommand c)
            {
                yield return new FakeEvent() { Id = c.Id };
            }
        }

        private class SubscribesToFakeEvent : SubscribesTo<FakeEvent>
        {
            public FakeEvent handled_event;
            public void Handle(FakeEvent @event)
            {
                handled_event = @event;
            }
        }

        private class StaticFakeEventSubscriber : SubscribesTo<FakeEvent>
        {
            public static FakeEvent handled_event;
            public void Handle(FakeEvent @event)
            {
                handled_event = @event;
            }
        }

        [Fact]
        public void SendCommand_WithRegisteredHandler_IsHandledByRegisteredHandler()
        {
            // Arrange
            var fakeMessageDispatcher = new EventSourcedMessageDispatcher(
                new FakeEventStore());
            var mockCommand = new FakeCommand();
            fakeMessageDispatcher
                .RegisteredHandlerFor<HandlesFakeCommand, FakeCommand>();

            // Act
            fakeMessageDispatcher.SendCommand(mockCommand);

            // Assert
            Assert.Equal(mockCommand, HandlesFakeCommand.handled_command);
        }

        [Fact]
        public void SendCommand_WithoutRegisteredHandler_ThrowsNoHandlerRegisteredException()
        {
            // Arrange
            var fakeMessageDispatcher = new EventSourcedMessageDispatcher(
                new FakeEventStore());
            var mockCommand = new FakeCommand();

            // Act
            Action sendUnhandledCommand = () => fakeMessageDispatcher
                .SendCommand(mockCommand);

            // Assert
            Assert.Throws<NoHandlerRegistered>(sendUnhandledCommand);
        }

        [Fact]
        public void RegisterHandlerFor_SameCommand_ThrowsCommandHandlerAlreadyRegisteredException()
        {
            // Arrange
            var fakeMessageDispatcher = new EventSourcedMessageDispatcher(
                new FakeEventStore());
            fakeMessageDispatcher
                .RegisteredHandlerFor<FirstFakeCommandHandler, FakeCommand>();

            // Act
            Action RegisterHandlerForSameCommand = () => fakeMessageDispatcher
                .RegisteredHandlerFor<SecondFakeCommandHandler, FakeCommand>();

            // Assert
            Assert.Throws<CommandHandlerAlreadyRegistered>(
                RegisterHandlerForSameCommand);
        }

        [Fact]
        public void SendCommand_WithStoredEvents_AppliesEventsBeforeHandlingCommand()
        {
            // Arrange
            var stubEventStore = new FakeEventStore();
            var mockEvent = new FakeEvent();
            stubEventStore.Save(new AdditionalEvents(Guid.NewGuid(),
                new ArrayList() { mockEvent }));
            var stubMessageDispatcher = new EventSourcedMessageDispatcher(stubEventStore);
            stubMessageDispatcher
                .RegisteredHandlerFor<AppliesFakeEventHandlesFakeCommand, FakeCommand>();
            var mockCommand = new FakeCommand();

            // Act
            stubMessageDispatcher.SendCommand(mockCommand);

            // Assert
            Assert.Equal(mockEvent,
                AppliesFakeEventHandlesFakeCommand.applied_event);
            Assert.Equal(mockCommand,
                AppliesFakeEventHandlesFakeCommand.handled_command);
        }

        [Fact]
        public void SendCommand_WithCommandThatEmitsEvent_SavesEventInEventStore()
        {
            // Arrange
            var stubEventStore = new FakeEventStore();
            var stubMessageDispatcher = new EventSourcedMessageDispatcher(stubEventStore);
            var mockGuid = Guid.NewGuid();
            stubMessageDispatcher
                .RegisteredHandlerFor<HandlesFakeCommandEmitsFakeEvents, FakeCommand>();

            // Act
            stubMessageDispatcher.SendCommand(
                new FakeCommand() { Id = mockGuid });

            // Assert
            Assert.Equal(mockGuid, ((FakeEvent)(stubEventStore
                .LoadEventsFor(mockGuid) as ArrayList)[0]).Id);
        }

        [Fact]
        public void RegisterSubscriberFor_EventEmittedFromCommand_HandlesCommand()
        {
            // Arrange
            var stubMessageDispatcher = new EventSourcedMessageDispatcher(
                new FakeEventStore());
            var mockSubscriber = new SubscribesToFakeEvent();
            var mockGuid = Guid.NewGuid();
            stubMessageDispatcher
                .RegisterSubscriberFor<FakeEvent>(mockSubscriber);
            stubMessageDispatcher
                .RegisteredHandlerFor<HandlesFakeCommandEmitsFakeEvents, FakeCommand>();

            // Act
            stubMessageDispatcher.SendCommand(
                new FakeCommand() { Id = mockGuid });

            // Assert
            Assert.Equal(mockGuid, mockSubscriber.handled_event.Id);
        }

        [Fact]
        public void ScanInstance_WithCommandHandler_RegistersCommandHandler()
        {
            // Arrange
            var stubMessageDispatcher = new EventSourcedMessageDispatcher(
                new FakeEventStore());
            var mockSubscriber = new SubscribesToFakeEvent();
            var mockGuid = Guid.NewGuid();
            stubMessageDispatcher
                .RegisterSubscriberFor<FakeEvent>(mockSubscriber);
            stubMessageDispatcher.ScanInstance(new HandlesFakeCommandEmitsFakeEvents());

            // Act
            stubMessageDispatcher.SendCommand(
                new FakeCommand() { Id = mockGuid });

            // Assert
            Assert.Equal(mockGuid, mockSubscriber.handled_event.Id);
        }

        [Fact]
        public void ScanInstance_WithEventSubscriber_RegistersEventSubscriber()
        {
            // Arrange
            var stubMessageDispatcher = new EventSourcedMessageDispatcher(
                new FakeEventStore());
            var mockGuid = Guid.NewGuid();
            stubMessageDispatcher.ScanInstance(new HandlesFakeCommandEmitsFakeEvents());
            stubMessageDispatcher.ScanInstance(new StaticFakeEventSubscriber());

            // Act
            stubMessageDispatcher.SendCommand(
                new FakeCommand() { Id = mockGuid });

            // Assert
            Assert.Equal(mockGuid, StaticFakeEventSubscriber.handled_event.Id);
        }
    }
}
