using FalandoSobre.Domain.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace FalandoSobre.Web.Components.Pages.CreateInstitution;

public class CreateInstitutionPage : ComponentBase
{
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;
    [Inject] protected NavigationManager? Navi { get; set; }
    [Inject] public IInstitutionRepository? InstitutionRepository { get; set; } = null!;
    [Inject] public required ILogger<CreateInstitutionPage> Logger { get; set; }
    [Inject] HttpClient Http { get; set; } = null!;

    protected string successMessage = string.Empty;
    protected string errorMessage = string.Empty;
    protected bool uploadInProgress = false;

    protected Institution Model { get; set; } = new();

    protected async Task CreateInstitution()
    {
        uploadInProgress = true;
        successMessage = string.Empty;
        errorMessage = string.Empty;

        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        try
        {
            var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == string.Empty || userIdString == null)
            {
                errorMessage = "Usuário não encontrado.";
            }
            Model.UserName = user.Identity!.Name!;
            Model.ApplicationUserId = userIdString!;
            var data = await InstitutionRepository!.AddAsync(Model);
            successMessage = "Parceria criada com sucesso!";
        }
        catch (Exception ex)
        {
            errorMessage = "Erro ao criar instituição: " + ex.Message;
            Logger.LogError(ex, "Erro ao criar instituição");
        }
        finally
        {
            uploadInProgress = false;
            Model = new();
            StateHasChanged();
        }
    }

    protected async Task HandleCepChange()
    {
        var cep = Model.Cep?.Replace("-", "").Replace(".", "").Trim();

        if (!string.IsNullOrEmpty(cep))
        {
            try
            {
                var addressInfo = await Http.GetFromJsonAsync<AddressResponse>($"https://viacep.com.br/ws/{cep}/json/");

                if (addressInfo != null && string.IsNullOrEmpty(addressInfo.erro))
                {
                    Model.Street = addressInfo.logradouro;
                    Model.Neighborhood = addressInfo.bairro;
                    Model.Uf = addressInfo.uf;
                    Model.City = addressInfo.localidade;
                }
                else
                {
                    errorMessage = "CEP inválido. Verifique e tente novamente.";
                }
            }
            catch (Exception)
            {
                errorMessage = "Erro ao buscar o CEP.";
            }
        }
        else
        {
            errorMessage = "Por favor, insira um CEP válido.";
        }
    }

    private class AddressResponse
    {
        public string logradouro { get; set; } = "";
        public string bairro { get; set; } = "";
        public string uf { get; set; } = "";
        public string localidade { get; set; } = "";
        public string erro { get; set; } = "";
    }
}
