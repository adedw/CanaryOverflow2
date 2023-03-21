using CanaryOverflow2.Domain.Common;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Testcontainers.EventStoreDb;

namespace CanaryOverflow2.Infrastructure.IntegrationTests.Fixtures;

[UsedImplicitly]
public class EventSourcedRepositoryFixture<TKey, TAggregate> : IAsyncLifetime
    where TKey : struct
    where TAggregate : AggregateRoot<TKey, TAggregate>
{
    private readonly EventStoreDbContainer _eventStoreDbContainer = new EventStoreDbBuilder().Build();

    public ServiceProvider? ServiceProvider { get; private set; }

    public async Task InitializeAsync()
    {
        await _eventStoreDbContainer.StartAsync().ConfigureAwait(false);

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddEventStoreClient(_eventStoreDbContainer.GetConnectionString());
        serviceCollection
            .AddTransient<IAggregateRepository<TKey, TAggregate>, AggregateRepository<TKey, TAggregate>>();

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    public async Task DisposeAsync()
    {
        await _eventStoreDbContainer.DisposeAsync().ConfigureAwait(false);
    }
}