using System.ComponentModel.DataAnnotations;
using FalandoSobre.DomainCore.Entities;

namespace FalandoSobre.Domain.Entities;

public class Report : Entity
{
    public Report()
    {
        //ORM Purpose
    }

    [Required(ErrorMessage = "O titulo da publicação é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome da publicação é no máximo 100 caracteres")]
    public string ReportName { get; set; }

    [Required(ErrorMessage = "O tipo de publicação é obrigatório")]
    public string TypeReport { get; set; }


    [Required(ErrorMessage = "O conteúdo da publicação é obrigatória")]
    [StringLength(2000, ErrorMessage = "O conteúdo é no máximo de 2000 caracteres")]
    public string ReportDescription { get; set; }

    public string UserName { get; set; }
    public bool? IsEvent { get; set; } = true;
    public DateTime ReportsDate { get; set; } = DateTime.Now;

    public virtual List<Comment> Comments { get; set; } = new List<Comment>();

    public virtual List<Like> Likes { get; set; } = new List<Like>();

    public virtual List<Image> Images { get; set; } = new List<Image>();

    public Report(string reportName, string typeReport, string reportDescription, DateTime reportDate, string userName, bool? isEvent) : base()
    {
        ReportName = reportName;
        TypeReport = typeReport;
        ReportDescription = reportDescription;
        ReportsDate = reportDate;
        UserName = userName;
        IsEvent = isEvent;
    }
}
