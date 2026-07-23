using Tanish.Domain.Commons;

namespace Tanish.Domain.Models;

public class AppUser : BaseEntity
{
    public long TelegramId { get; set; }
    public string Alias { get; set; }
    public bool IsActive { get; set; }

    public ICollection<ActivityProfile> Profiles { get; set; } = new List<ActivityProfile>();

}
