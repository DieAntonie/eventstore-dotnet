using System;
using System.Collections;

namespace Core
{
    public interface EventStore
    {
        ICollection LoadEventsFor(Guid aggregateId);
        void Save(AdditionalEvents additionalEvents);
    }
}