using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;

namespace Core
{
    public class InMemoryEventStore : EventStore
    {
        private class Stream
        {
            public ArrayList Events;
        }

        private ConcurrentDictionary<Guid, Stream> store =
            new ConcurrentDictionary<Guid, Stream>();

        public ICollection LoadEventsFor(Guid aggregateId)
        {
            Stream stream;
            if (store.TryGetValue(aggregateId, out stream))
                return stream.Events;
            else
                return new ArrayList();
        }

        public void Save(AdditionalEvents additionalEvents)
        {
            var stream = store.GetOrAdd(additionalEvents.AggregateId, _ => new Stream());
            ArrayList eventList, newEventList;
            do
            {
                eventList = stream.Events;
                if (IsRaceHazard(additionalEvents, eventList))
                    throw new ConcurrencyConflictOccurred();
                newEventList = eventList == null ? new ArrayList() : (ArrayList)eventList.Clone();
                newEventList.AddRange(additionalEvents.Events);
            } while (Interlocked.CompareExchange(
                ref stream.Events, newEventList, eventList) != eventList);
        }

        private static bool IsRaceHazard(AdditionalEvents additionalEvents, ArrayList eventList) =>
            (eventList == null ? 0 : eventList.Count) != additionalEvents.PrecedingEventCount;
    }
}
