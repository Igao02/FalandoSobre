
using FalandoSobre.Api.Extensions;
using FalandoSobre.Api.Infrastructure;
using FalandoSobre.SharedKernel;
using FalandoSobreApplication.UseCases.InstitutionUseCase.Create;
using MediatR;

namespace FalandoSobre.Api.Endpoints.Institution;

public sealed class CreateInstitutionEndpoint : IEndpoint
{
    public sealed record Request(string CorporateName, 
        string Document, 
        string Cep, 
        string City, 
        string Street, 
        int NumHome, 
        string Neighborhood, 
        string Uf, 
        string UserName, 
        string Complement, 
        string ApplicationUserId, 
        bool Actived);

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/institutions/create", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateInstitutionCommand(
                request.CorporateName,
                request.Document,
                request.Cep,
                request.City,
                request.Street,
                request.Neighborhood,
                request.Uf,
                request.NumHome,
                request.Complement,
                request.UserName,
                request.ApplicationUserId,
                request.Actived);
            Result<CreateInstitutionResponse> result = await sender.Send(command, cancellationToken);
            return result.Match(
                value => Results.Ok(value),
                CustomResults.SimpleError
            );
        })
        .WithName("CreateInstitution")
        .WithTags("Institutions")
        .WithOpenApi();
    }
}
