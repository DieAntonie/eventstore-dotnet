using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;

namespace Core
{
    public class EventSourcedMessageDispatcher : MessageDispatcher
    {
        private Dictionary<Type, Action<object>> commandHandlers =
            new Dictionary<Type, Action<object>>();
            
        private Dictionary<Type, List<Action<object>>> eventSubscribers =
            new Dictionary<Type, List<Action<object>>>();

        private EventStore store;

        public EventSourcedMessageDispatcher(EventStore eventStore)
        {
            store = eventStore;
        }

        public void SendCommand<TCommand>(TCommand command)
        {
            if (!commandHandlers.ContainsKey(typeof(TCommand)))
                throw new NoHandlerRegistered();

            commandHandlers[typeof(TCommand)](command);
        }

        public void RegisteredHandlerFor<TAggregate, TCommand>()
            where TAggregate : Aggregate, new()
        {
            if (commandHandlers.ContainsKey(typeof(TCommand)))
                throw new CommandHandlerAlreadyRegistered();

            commandHandlers.Add(typeof(TCommand),
                GenerateCommandHandlerAction<TAggregate, TCommand>());
        }

        private Action<object> GenerateCommandHandlerAction<TAggregate, TCommand>()
            where TAggregate : Aggregate, new() => command =>
        {
            var historicalEvents = store.LoadEventsFor((command as CommandMessage).Id);
            var eventCount = historicalEvents.Count;

            foreach (var resultingEvent in
                HydratedCommandHandler<TAggregate, TCommand>(
                    (command as CommandMessage).Id,
                    historicalEvents
                ).Handle((TCommand)command))
                Publish((command as CommandMessage).Id, resultingEvent, eventCount);
        };

        private static Handles<TCommand> HydratedCommandHandler<TAggregate, TCommand>(
            Guid aggregateId, ICollection historicalEvents)
            where TAggregate : Aggregate, new()
        {
            var aggregate = new TAggregate() { Id = aggregateId };
            aggregate.ApplyEvents(historicalEvents);
            return (aggregate as Handles<TCommand>);
        }

        private void Publish(
            Guid aggregateId, object @event, int precedingEventCount)
        {
            store.Save(new AdditionalEvents(aggregateId,
                new ArrayList() { @event }, precedingEventCount));
                
            if (eventSubscribers.ContainsKey(@event.GetType()))
                foreach (var subscriber in eventSubscribers[@event.GetType()])
                    subscriber(@event);
        }

        public void RegisterSubscriberFor<TEvent>(SubscribesTo<TEvent> subscriber)
        {
            if (!eventSubscribers.ContainsKey(typeof(TEvent)))
                eventSubscribers.Add(typeof(TEvent), new List<Action<object>>());

            eventSubscribers[typeof(TEvent)].Add(@event =>
                subscriber.Handle((TEvent)@event));
        }

        public void ScanAssembly(Assembly ass)
        {
            // // Scan for and register handlers.
            // var handlers = 
            //     from t in ass.GetTypes()
            //     from i in t.GetInterfaces()
            //     where i.IsGenericType
            //     where i.GetGenericTypeDefinition() == typeof(IHandleCommand<>)
            //     let args = i.GetGenericArguments()
            //     select new
            //     {
            //         CommandType = args[0],
            //         AggregateType = t
            //     };
            // foreach (var h in handlers)
            //     this.GetType().GetMethod("AddHandlerFor") 
            //         .MakeGenericMethod(h.CommandType, h.AggregateType)
            //         .Invoke(this, new object[] { });

            // // Scan for and register subscribers.
            // var subscriber =
            //     from t in ass.GetTypes()
            //     from i in t.GetInterfaces()
            //     where i.IsGenericType
            //     where i.GetGenericTypeDefinition() == typeof(ISubscribeTo<>)
            //     select new
            //     {
            //         Type = t,
            //         EventType = i.GetGenericArguments()[0]
            //     };
            // foreach (var s in subscriber)
            //     this.GetType().GetMethod("RegisterSubscriber")
            //         .MakeGenericMethod(s.EventType)
            //         .Invoke(this, new object[] { CreateInstanceOf(s.Type) });
        }

        public void ScanInstance(object instance)
        {
            foreach (var details in handlerDetailsFrom(instance))
                RegisterGenericHandler(details.AggregateType, details.CommandType);

            foreach (var subscribedEvent in subscribedEventsFrom(instance))
                RegisterGenericSubscriber(subscribedEvent, instance);
        }

        private static IEnumerable<CommandHandlerDetails> handlerDetailsFrom(
            object instance) =>
            from i in instance.GetType().GetInterfaces()
            where i.IsGenericType
            where i.GetGenericTypeDefinition() == typeof(Handles<>)
            let args = i.GetGenericArguments()
            select new CommandHandlerDetails()
            {
                CommandType = args[0],
                AggregateType = instance.GetType()
            };

        private void RegisterGenericHandler(Type aggregateType, Type commandType)
        {
            this.GetType().GetMethod("RegisteredHandlerFor")
                .MakeGenericMethod(aggregateType, commandType)
                .Invoke(this, new object[] { });
        }

        private static IEnumerable<Type> subscribedEventsFrom(
            object instance) =>
            from i in instance.GetType().GetInterfaces()
            where i.IsGenericType
            where i.GetGenericTypeDefinition() == typeof(SubscribesTo<>)
            select i.GetGenericArguments()[0];

        private void RegisterGenericSubscriber(Type eventType, object instance)
        {
            this.GetType().GetMethod("RegisterSubscriberFor")
                .MakeGenericMethod(eventType)
                .Invoke(this, new object[] { instance });
        }
    }

    internal class CommandHandlerDetails
    {
        public Type CommandType;
        public Type AggregateType;
    }
}