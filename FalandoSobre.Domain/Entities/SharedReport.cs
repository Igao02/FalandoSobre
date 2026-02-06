using FalandoSobre.DomainCore.Entities;
using System.ComponentModel.DataAnnotations;

namespace FalandoSobre.Domain.Entities;

public class SharedReport : Entity
{
    public SharedReport()
    {
        //ORM Purpose
    }

    public virtual Guid ReportId { get; set; }

    public virtual Report? Report { get; set; }

    public string? ApplicationUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public bool Actived { get; set; } = true;

    public string UserName { get; set; } = string.Empty;

    public SharedReport(Guid reportId, string applicationUserId, string userName) : base()
    {
        ReportId = reportId;
        ApplicationUserId = applicationUserId;
        UserName = userName;
    }
}
