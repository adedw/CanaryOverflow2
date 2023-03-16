using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using CommunityToolkit.Diagnostics;

namespace CanaryOverflow2.Domain.Common;

public abstract class AggregateRoot<TKey, TAggregate> : Entity<TKey>
    where TAggregate : AggregateRoot<TKey, TAggregate>
    where TKey : struct
{
    private static readonly Func<TAggregate> Constructor;

    static AggregateRoot()
    {
        var newExpression = Expression.New(typeof(TAggregate));
        var lambdaExpression = Expression.Lambda<Func<TAggregate>>(newExpression);
        Constructor = lambdaExpression.Compile();
    }

    public static TAggregate Create(IReadOnlyCollection<IDomainEvent> events)
    {
        Guard.HasSizeGreaterThan(events, 0);

        var aggregate = Constructor.Invoke();

        foreach (var @event in events)
        {
            aggregate.When(@event);
            aggregate.Version++;
        }

        return aggregate;
    }


    private readonly Queue<IDomainEvent> _events;

    protected AggregateRoot()
    {
        _events = new Queue<IDomainEvent>();
    }

    public int Version { get; private set; }

    protected abstract void When(IDomainEvent @event);

    protected void Append(IDomainEvent @event)
    {
        _events.Enqueue(@event);

        Version++;

        When(@event);
    }

    public IReadOnlyCollection<IDomainEvent> GetUncommittedEvents() => _events;
    public void ClearUncommittedEvents() => _events.Clear();
}