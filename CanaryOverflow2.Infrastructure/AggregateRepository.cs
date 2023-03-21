using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using CanaryOverflow2.Domain.Common;

using CommunityToolkit.Diagnostics;

using EventStore.Client;

using JetBrains.Annotations;

namespace CanaryOverflow2.Infrastructure;

[UsedImplicitly]
public class AggregateRepository<TKey, TAggregate> : IAggregateRepository<TKey, TAggregate>
    where TAggregate : AggregateRoot<TKey, TAggregate>
    where TKey : struct
{
    private static readonly string StreamPrefix = typeof(TAggregate).Name;
    private static string GetStreamName(TKey key) => $"{StreamPrefix}-{key}";

    private readonly EventStoreClient _eventStoreClient;

    public AggregateRepository(EventStoreClient eventStoreClient)
    {
        _eventStoreClient = eventStoreClient;
    }

    public async Task SaveAsync(TAggregate? aggregate, CancellationToken cancellationToken = default)
    {
        Guard.IsNotNull(aggregate);

        var events = aggregate.GetUncommittedEvents();

        if (events.Count < 1) return;

        var streamName = GetStreamName(aggregate.Id);
        var expectedRevision = StreamRevision.FromInt64(aggregate.Version - events.Count - 1);
        var eventData = events.Select(AsEventData);

        await _eventStoreClient.AppendToStreamAsync(streamName, expectedRevision, eventData,
            cancellationToken: cancellationToken);

        aggregate.ClearUncommittedEvents();
    }

    private static EventData AsEventData(IDomainEvent @event)
    {
        var data = JsonSerializer.SerializeToUtf8Bytes(@event as object);
        var type = @event.GetType().AssemblyQualifiedName;
        Guard.IsNotNull(type);
        
        return new EventData(
            eventId: Uuid.NewUuid(),
            type: type,
            data: new ReadOnlyMemory<byte>(data)
        );
    }

    public async Task<TAggregate> FindAsync(TKey key, CancellationToken cancellationToken = default)
    {
        var streamName = GetStreamName(key);

        var events = await _eventStoreClient
            .ReadStreamAsync(Direction.Forwards, streamName, StreamPosition.Start, cancellationToken: cancellationToken)
            .Select(AsDomainEvent)
            .Where(e => e is not null)
            .Select(e => e!)
            .ToListAsync(cancellationToken);

        return AggregateRoot<TKey, TAggregate>.Create(events);
    }

    private static IDomainEvent? AsDomainEvent(ResolvedEvent @event)
    {
        var data = @event.Event.Data.Span;
        var type = Type.GetType(@event.Event.EventType);
        Guard.IsNotNull(type);

        return JsonSerializer.Deserialize(data, type) as IDomainEvent;
    }
}