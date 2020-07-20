using System;
using System.Collections;

namespace Core
{
    public interface Aggregate
    {
        Guid Id { get; set; }

        void ApplyEvents(IEnumerable events);

        void ApplyEvent<TEvent>(TEvent @event);
    }
}