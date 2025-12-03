using FalandoSobre.DomainCore.Entities;
using System.ComponentModel.DataAnnotations;


namespace FalandoSobre.Domain.Entities;

public class Comment : Entity
{
    public Comment()
    {
        //ORM Purpose
    }

    [Required(ErrorMessage = "Conteúdo do comentário é necessário")]
    public string CommentContent { get; set; }

    public DateTime CommentDate { get; set; } = DateTime.Now;

    public bool Actived { get; set; } = true;

    public string UserName { get; set; }

    public virtual Guid ReportId { get; set; }

    public virtual Report Report { get; set; }

    public string? ApplicationUserId { get; set; }

    public Comment(string commentContent, DateTime commentDate, Guid reportId, string userName, string? applicationUserId) : base()
    {
        CommentContent = commentContent;
        CommentDate = commentDate;
        ReportId = reportId;
        UserName = userName;
        ApplicationUserId = applicationUserId;
    }
}

