using FalandoSobre.DomainCore.Entities;

namespace FalandoSobre.Domain.Entities;

public class Image : Entity
{
    public Image() 
    {
        //ORM Purpose
    }

    public string ImageUrl { get; set; } = string.Empty;

    public byte[]? ConteudoArquivo { get; set; }

    public DateTime ImageDate { get; set; }

    public virtual Guid ReportId { get; set; }

    public virtual Report? Report { get; set; }

    public Image(string imageUrl, byte[]? conteudoArquivo, DateTime imageDate, Guid reportId) : base()
    {
        ImageUrl = imageUrl;
        ImageDate = imageDate;
        ReportId = reportId;
        ConteudoArquivo = conteudoArquivo;
    }
}
