using FalandoSobre.DomainCore.Entities;

namespace FalandoSobre.Domain.Entities;

public class Like : Entity
{
    public Like()
    {
        //ORM Purpose
    }

    public DateTime? LikeDate { get; set; } = DateTime.Now;

    public virtual Guid ReportId { get; set; }

    public virtual Report Report { get; set; }

    public bool Actived { get; set; } = true;

    public string? ApplicationUserId { get; set; }

    public Like(DateTime? likeDate, Guid reportId, string? applicationUserId) : base()
    {
        LikeDate = likeDate;
        ReportId = reportId;
        ApplicationUserId = applicationUserId;
    }
}
