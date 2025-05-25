namespace FalandoSobreApplication.UseCases.InstitutionUseCase.Create;

public sealed class CreateInstitutionResponse
{
    public Guid Id { get; init; }
    public string CorporateName { get; init; } = default!;
    public string Document { get; init; } = default!;
    public string Cep { get; init; } = default!;
    public string City { get; init; } = default!;
    public string Street { get; init; } = default!;
    public string Neighborhood { get; init; } = default!;
    public string Uf { get; init; } = default!;
    public int NumHome { get; init; }
    public string UserName { get; init; } = default!;
    public string Complement { get; init; } = default!;
    public DateTime CreationDate { get; init; }
    public string ApplicationUserId { get; init; } = default!;
    public bool Actived { get; init; } = default!;
}
