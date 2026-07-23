namespace Tanish.Domain.Commons;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }

}
