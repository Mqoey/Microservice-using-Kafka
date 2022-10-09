using CQRS.Core.Events;

namespace Post.Common.Events
{
  public class CommentRemoveEvent : BaseEvent
  {
    public CommentRemoveEvent() : base(nameof(CommentRemoveEvent))
    {
    }
    public string CommentId { get; set; }
  }
}