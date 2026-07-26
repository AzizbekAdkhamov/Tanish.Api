using Tanish.Domain.Commons;
using Tanish.Domain.Models.Profile;

namespace Tanish.Domain.Models.User;

public class AppUser : BaseEntity
{
    public long TelegramId { get; set; }
    public string Alias { get; set; }
    public bool IsActive { get; set; }
    public string? TelegramPhotoFileId { get; set; }

    public ICollection<ActivityProfile> Profiles { get; set; } = new List<ActivityProfile>();

}
