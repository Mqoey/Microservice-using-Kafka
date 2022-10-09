using CQRS.Core.Domains;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates
{
  public class PostAggregate : AggregateRoot
  {
    private bool _active;
    private string _author;
    private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

    public bool Active
    {
      get => _active; set => _active = value;
    }
    public PostAggregate()
    {
    }
    public PostAggregate(Guid id, string author, string message)
    {
      RaiseEvent(new PostCreatedEvent
      {
        Id = id,
        Author = author,
        Message = message,
        DatePosted = DateTime.Now
      });
    }
    public void Apply(PostCreatedEvent @event)
    {
      _id = @event.Id;
      _author = @event.Author;
      _active = true;
    }
    public void EditMessage(string message)
    {
      if (!_active)
      {
        throw new InvalidOperationException("Cannot edit a post that is not active");
      }
      if (string.IsNullOrWhiteSpace(message))
      {
        throw new InvalidOperationException($"The value of {nameof(message)} cannot be null or empty. Please provide a valid {nameof(message)}!");
      }
      RaiseEvent(new MessageUpdatedEvent
      {
        Id = _id,
        Message = message
      });
    }
    public void Apply(MessageUpdatedEvent @event)
    {
      _id = @event.Id;
    }
    public void LikePost()
    {
      if (!_active)
      {
        throw new InvalidOperationException("Cannot like a post that is not active");
      }
      RaiseEvent(new PostLikedEvent
      {
        Id = _id,
      });
    }
    public void Apply(PostLikedEvent @event)
    {
      _id = @event.Id;
    }
    public void AddComment(string comment, string username)
    {
      if (!_active)
      {
        throw new InvalidOperationException("Cannot comment on a post that is not active");
      }
      if (string.IsNullOrWhiteSpace(comment))
      {
        throw new InvalidOperationException($"The value of {nameof(comment)} cannot be null or empty. Please provide a valid {nameof(comment)}!");
      }
      if (string.IsNullOrWhiteSpace(username))
      {
        throw new InvalidOperationException($"The value of {nameof(username)} cannot be null or empty. Please provide a valid {nameof(username)}!");
      }
      var commentId = Guid.NewGuid();
      RaiseEvent(new CommentAddedEvent
      {
        Id = _id,
        CommentId = commentId,
        Comment = comment,
        Username = username
      });
    }
    public void Apply(CommentAddedEvent @event)
    {
      _id = @event.Id;
      _comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.Username));
    }
    public void EditComment(Guid commentId, string comment, string username)
    {
      if (!_active)
      {
        throw new InvalidOperationException("Cannot edit a comment on a post that is not active");
      }
      if (string.IsNullOrWhiteSpace(comment))
      {
        throw new InvalidOperationException($"The value of {nameof(comment)} cannot be null or empty. Please provide a valid {nameof(comment)}!");
      }
      if (string.IsNullOrWhiteSpace(username))
      {
        throw new InvalidOperationException($"The value of {nameof(username)} cannot be null or empty. Please provide a valid {nameof(username)}!");
      }
      if (!_comments.ContainsKey(commentId))
      {
        throw new InvalidOperationException($"The comment with id {commentId} does not exist!");
      }
      if (_comments[commentId].Item2 != username)
      {
        throw new InvalidOperationException($"The user {username} is not the author of the comment with id {commentId}!");
      }
      RaiseEvent(new CommentUpdatedEvent
      {
        Id = _id,
        CommentId = commentId,
        Comment = comment,
        Username = username,
        EditDate = DateTime.Now
      });
    }
    public void RemoveComment(Guid commentId, string username)
    {
      if (!_active)
      {
        throw new InvalidOperationException("Cannot remove a comment on a post that is not active");
      }
      if (string.IsNullOrWhiteSpace(username))
      {
        throw new InvalidOperationException($"The value of {nameof(username)} cannot be null or empty. Please provide a valid {nameof(username)}!");
      }
      if (!_comments.ContainsKey(commentId))
      {
        throw new InvalidOperationException($"The comment with id {commentId} does not exist!");
      }
      if (_comments[commentId].Item2 != username)
      {
        throw new InvalidOperationException($"The user {username} is not the author of the comment with id {commentId}!");
      }
      RaiseEvent(new CommentRemovedEvent
      {
        Id = _id,
        CommentId = commentId
      });
    }
    public void Apply(CommentUpdatedEvent @event)
    {
      _id = @event.Id;
      _comments[@event.CommentId] = new Tuple<string, string>(@event.Comment, @event.Username);
    }
    public void DeletePost(string username)
    {
      if (!_active)
      {
        throw new InvalidOperationException("Cannot delete a post that is not active");
      }
      if (!_author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
      {
        throw new InvalidOperationException($"The user {username} is not the author of the post with id {_id}!");
      }
      RaiseEvent(new PostDeletedEvent
      {
        Id = _id,
      });
    }
    public void Apply(PostDeletedEvent @event)
    {
      _id = @event.Id;
      _active = false;
    }
  }
}