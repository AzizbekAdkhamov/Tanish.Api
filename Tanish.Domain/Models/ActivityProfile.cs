using Pgvector;
using Tanish.Domain.Commons;
using Tanish.Domain.Enums;

namespace Tanish.Domain.Models;
public class ActivityProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public AppUser User { get; set; }

    public ActivityCategory Category { get; set; }
    public ExperienceLevel Level { get; set; }
    public string Availability { get;set; }
    public string BlurbText { get; set; }
    public Vector BlurbEmbredding { get; set; }
    public bool IsSearchable { get; set; }
}
