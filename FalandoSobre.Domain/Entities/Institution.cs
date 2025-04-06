using FalandoSobre.Domain.Constants;
using FalandoSobre.DomainCore.Entities;
using System.ComponentModel.DataAnnotations;

public class Institution : Entity
{
    public Institution()
    {
        //ORM Purpose
    }

    [Required(ErrorMessage = "O nome da corporação é obrigatório")]
    [StringLength(LenghtConst.MaxName, ErrorMessage = "O máximo de caracteres é 150")]
    public string CorporateName { get; set; } = "";

    [Required(ErrorMessage = "O documento é obrigatório")]
    [StringLength(LenghtConst.MaxDocNumber, ErrorMessage = "O máximo de caracteres é 14")]
    public string Document { get; set; } = "";

    [Required(ErrorMessage = "O CEP e obrigatório")]
    [StringLength(LenghtConst.MaxCep, ErrorMessage = "O máximo de caracteres é 10")]
    public string Cep { get; set; } = "";

    [Required(ErrorMessage = "A cidade é obrigatória")]
    [StringLength(LenghtConst.MaxName, ErrorMessage = "O máximo de caracteres é 150")]
    public string City { get; set; } = "";

    [Required(ErrorMessage = "A rua é obrigatória")]
    [StringLength(LenghtConst.MaxAddName, ErrorMessage = "O máximo de caracteres é 150")]
    public string Street { get; set; } = "";

    [Required(ErrorMessage = "O bairro é obrigatório")]
    [StringLength(LenghtConst.MaxName, ErrorMessage = "O máximo de caracteres é 150")]
    public string Neighborhood { get; set; } = "";

    [Required(ErrorMessage = "A sigla do estado é obrigatório")]
    [StringLength(LenghtConst.NumUf, ErrorMessage = "O máximo de caracteres é 2")]
    public string Uf { get; set; } = "";

    [Required(ErrorMessage = "O número do endereço é obrigatório")]
    public int NumHome { get; set; } = 0;

    [StringLength(LenghtConst.MaxName, ErrorMessage = "O máximo de caracteres é 150")]
    public string? Complement { get; set; } = "";

    public DateTime? CreationDate { get; set; } = DateTime.Now;

    public string UserName { get; set; }

    public Institution(string corporateName, string document, string cep, string street, int numHome, string? complement, DateTime? creationDate, string userName, string neighborhood, string uf) : base()
    {
        CorporateName = corporateName;
        Document = document;
        Cep = cep;
        Street = street;
        NumHome = numHome;
        CreationDate = creationDate;
        UserName = userName;
        Complement = complement;
        Neighborhood = neighborhood;
        Uf = uf;
    }
}