using System.Threading;
using System.Threading.Tasks;

namespace CanaryOverflow2.Domain.Common;

public interface IAggregateRepository<in TKey, TAggregate>
    where TAggregate : AggregateRoot<TKey, TAggregate>
    where TKey : struct
{
    Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken = default);
    Task<TAggregate> FindAsync(TKey key, CancellationToken cancellationToken = default);
}