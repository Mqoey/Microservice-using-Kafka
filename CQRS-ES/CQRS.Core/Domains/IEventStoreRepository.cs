using CQRS.Core.Events;

namespace CQRS.Core.Domains
{
  public interface IEventStoreRepository
  {
    Task SaveAsync(EventModel @event);
    Task<List<EventModel>> FindByAggregateId(Guid aggregateId);
  }
}