using CQRS.Core.Events;

namespace CQRS.Core.Domains;
public abstract class AggregateRoot
{
  protected Guid _id;
  private readonly List<BaseEvent> _changes = new();
  public Guid Id
  {
    get { return _id; }
  }

  public int Version { get; set; } = -1;

  public IEnumerable<BaseEvent> GetUncommittedChanges()
  {
    return _changes;
  }

  public void MarkChangesAsCommitted()
  {
    _changes.Clear();
  }

  private void ApplyChanges(BaseEvent @event, bool isNew)
  {
    var method = this.GetType().GetMethod("Apply", new Type[] { @event.GetType() });

    if (method == null)
    {
      throw new ArgumentNullException("method", string.Format("No method found to apply event {0} to aggregate {1}", @event.GetType().Name, this.GetType().Name));
    }
    method.Invoke(this, new object[] { @event });

    if (isNew)
    {
      _changes.Add(@event);
    }
  }
  protected void RaiseEvent(BaseEvent @event)
  {
    ApplyChanges(@event, true);
  }

  public void ReplyEvents(IEnumerable<BaseEvent> events)
  {
    foreach (var baseevent in events.OrderBy(baseevent => baseevent.Version))
    {
      ApplyChanges(baseevent, false);
    }
  }
}