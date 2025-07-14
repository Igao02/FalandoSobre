using FalandoSobre.Web.Components.Account;
using FalandoSobre.Web.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FalandoSobre.Web.Components.Pages.Profile;

public class EditProfilePage : ComponentBase
{
    protected ApplicationUser user = default!;
    protected string? username;
    protected string? phoneNumber;

    protected InputModel Model { get; set; } = new();

    [Inject] IdentityUserAccessor UserAccessor { get; set; } = default!;
    [Inject] protected UserManager<ApplicationUser> UserManager { get; set; } = default!;
    [Inject] protected SignInManager<ApplicationUser> SignInManager { get; set; } = default!;
    [Inject] IdentityRedirectManager RedirectManager { get; set; } = default!;
    [Inject] protected IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var httpContext = HttpContextAccessor.HttpContext;

        user = await UserAccessor.GetRequiredUserAsync(httpContext!);
        username = await UserManager.GetUserNameAsync(user);
        phoneNumber = await UserManager.GetPhoneNumberAsync(user);

        Model.PhoneNumber ??= phoneNumber;
    }

    protected async Task OnValidSubmitAsync()
    {
        if (Model.PhoneNumber != phoneNumber)
        {
            var setPhoneResult = await UserManager.SetPhoneNumberAsync(user, Model.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                RedirectManager.RedirectToCurrentPageWithStatus("Erro ao atualizar o número de telefone.", HttpContextAccessor.HttpContext!);
                return;
            }
        }

        // Atualize a sessão de login primeiro
        await SignInManager.RefreshSignInAsync(user);

        //  Só depois redirecione
        RedirectManager.RedirectToCurrentPageWithStatus("Perfil atualizado com sucesso.", HttpContextAccessor.HttpContext!);
    }


    public class InputModel
    {
        [Phone(ErrorMessage = "Número de telefone inválido")]
        [Display(Name = "Número de telefone")]
        public string? PhoneNumber { get; set; }
    }
}
