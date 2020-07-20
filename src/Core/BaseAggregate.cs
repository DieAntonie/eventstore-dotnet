using System;
using System.Collections;
using System.Reflection;

namespace Core
{
    public class BaseAggregate : Aggregate
    {
        public Guid Id { get; set;}

        public void ApplyEvents(IEnumerable events)
        {
            foreach (var @event in events)
                GenericApplyEvent(@event);
        }

        private void GenericApplyEvent(object @event)
        {
            var genericApplyEventMethod = GetGenericApplyEventFor(@event);
            TryInvokeWith(genericApplyEventMethod, new object[] { @event });
        }

        private MethodInfo GetGenericApplyEventFor(object @event)
        {
            var aggregateType = this.GetType();
            var applyEventMethodInfo = aggregateType.GetMethod("ApplyEvent");
            var genericApplyEvent = applyEventMethodInfo.MakeGenericMethod(@event.GetType());
            return genericApplyEvent;
        }

        private void TryInvokeWith(MethodInfo genericApplyEvent, object[] parameters)
        {
            try
            {
                genericApplyEvent.Invoke(this, parameters);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        public void ApplyEvent<TEvent>(TEvent @event)
        {
            if (!IsApplicableTo<TEvent>())
                throw new UnapplicableEvent();

            (this as Applies<TEvent>).Apply(@event);
            // EventsLoaded++;
        }

        private bool IsApplicableTo<TEvent>() =>
            typeof(Applies<TEvent>).IsAssignableFrom(this.GetType());
    }
}
