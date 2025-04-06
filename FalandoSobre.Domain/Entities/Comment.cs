using FalandoSobre.DomainCore.Entities;
using System.ComponentModel.DataAnnotations;

namespace FalandoSobre.Domain.Entities;

public class Comment : Entity
{
    public Comment()
    {
        //Empty
    }

    [Required(ErrorMessage = "Conteúdo do comentário é necessário")]
    public string CommentContent { get; set; }

    public DateTime CommentDate { get; set; } = DateTime.Now;

    public string UserName { get; set; }

    public virtual Guid ReportId { get; set; }

    public virtual Report Report { get; set; }

    public Comment(string commentContent, DateTime commentDate, Guid reportId, string userName) : base()
    {
        CommentContent = commentContent;
        CommentDate = commentDate;
        ReportId = reportId;
        UserName = userName;
    }
}

