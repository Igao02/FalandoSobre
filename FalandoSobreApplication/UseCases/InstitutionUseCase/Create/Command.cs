
using Application.Abstractions.Messaging;

namespace FalandoSobreApplication.UseCases.InstitutionUseCase.Create;

public sealed record CreateInstitutionCommand(
    string CorporateName, 
    string Document, 
    string Cep, 
    string City, 
    string Street, 
    string Neighborhood, 
    string Uf, 
    int NumHome, 
    string Complement,
    string UserName, 
    string ApplicationUserId, 
    bool Actived
) : ICommand<CreateInstitutionResponse>;
