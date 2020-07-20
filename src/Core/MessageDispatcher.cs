using System.Reflection;

namespace Core
{
    public interface MessageDispatcher
    {
        void SendCommand<TCommand>(TCommand command);

        void RegisteredHandlerFor<TAggregate, TCommand>()
            where TAggregate : Aggregate, new();

        void RegisterSubscriberFor<TEvent>(SubscribesTo<TEvent> subscriber);

        void ScanAssembly(Assembly ass);

        void ScanInstance(object instance);
    }
}