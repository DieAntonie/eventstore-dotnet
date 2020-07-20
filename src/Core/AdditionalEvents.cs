using System;
using System.Collections;

namespace Core
{
    public class AdditionalEvents
    {
        public AdditionalEvents(Guid aggregateId,
                                ArrayList events,
                                int precedingEventCount = 0)
        {
            AggregateId = aggregateId;
            Events = events;
            PrecedingEventCount = precedingEventCount;
        }

        public Guid AggregateId { get; }

        public ArrayList Events { get; }

        public int PrecedingEventCount { get; }
    }
}