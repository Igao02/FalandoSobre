using FalandoSobre.Web.Components.Account;
using FalandoSobre.Web.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace FalandoSobre.Web.Components.Pages.Profile;

public class EditProfilePage : ComponentBase
{
    protected ApplicationUser user = default!;
    protected string? username;
    protected string? phoneNumber;
    protected string? email;
    protected bool isEmailConfirmed;
    protected string? message;

    protected InputModel Model { get; set; } = new();

    [Inject] IdentityUserAccessor UserAccessor { get; set; } = default!;
    [Inject] protected UserManager<ApplicationUser> UserManager { get; set; } = default!;
    [Inject] protected SignInManager<ApplicationUser> SignInManager { get; set; } = default!;
    [Inject] IdentityRedirectManager RedirectManager { get; set; } = default!;
    [Inject] protected IHttpContextAccessor HttpContextAccessor { get; set; } = default!;
    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject] protected IEmailSender<ApplicationUser> EmailSender { get; set; } = default!;


    protected override async Task OnInitializedAsync()
    {
        var httpContext = HttpContextAccessor.HttpContext;

        user = await UserAccessor.GetRequiredUserAsync(httpContext!);
        username = await UserManager.GetUserNameAsync(user);
        phoneNumber = await UserManager.GetPhoneNumberAsync(user);
        email = await UserManager.GetEmailAsync(user);
        isEmailConfirmed = await UserManager.IsEmailConfirmedAsync(user);

        Model.PhoneNumber ??= phoneNumber;
        Model.UserName ??= username;
    }

    protected async Task OnValidSubmitAsync()
    {
        if (Model.PhoneNumber != phoneNumber)
        {
            var setPhoneResult = await UserManager.SetPhoneNumberAsync(user, Model.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                RedirectManager.RedirectToCurrentPageWithStatus("Error: Failed to set phone number.", HttpContextAccessor.HttpContext);
                return;
            }
        }

        if (Model.NewEmail is not null && Model.NewEmail != email)
        {
            var userId = await UserManager.GetUserIdAsync(user);
            var code = await UserManager.GenerateChangeEmailTokenAsync(user, Model.NewEmail);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = NavigationManager.GetUriWithQueryParameters(
                NavigationManager.ToAbsoluteUri("Account/ConfirmEmailChange").AbsoluteUri,
                new Dictionary<string, object?> { ["userId"] = userId, ["email"] = Model.NewEmail, ["code"] = code });

            await EmailSender.SendConfirmationLinkAsync(user, Model.NewEmail, callbackUrl);

            message = "Confirmation link to change email sent. Please check your email.";
        }

        Console.WriteLine($"Username: {Model.UserName}, Current Username: {username}");

        if (Model.UserName is not null && Model.UserName != username)
        {
            if (!Regex.IsMatch(Model.UserName, @"^[a-zA-Z0-9\s_-]+$"))
            {
                message = "Apelido inválido. Use apenas letras, números, espaços, hífens e sublinhados.";
                return;
            }

            var setUserNameResult = await UserManager.SetUserNameAsync(user, Model.UserName);
            if (!setUserNameResult.Succeeded)
            {
                RedirectManager.RedirectToCurrentPageWithStatus("Error: Failed to set username.", HttpContextAccessor.HttpContext);
                return;
            }
        }

        await JSRuntime.InvokeVoidAsync("window.location.reload");
    }

    protected async Task OnSendEmailVerificationAsync()
    {
        if (email is null)
        {
            return;
        }

        var userId = await UserManager.GetUserIdAsync(user);
        var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = NavigationManager.GetUriWithQueryParameters(
            NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
            new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code });

        await EmailSender.SendConfirmationLinkAsync(user, Model.NewEmail, callbackUrl);

        message = "Verification email sent. Please check your email.";
    }

    public class InputModel
    {
        [Phone(ErrorMessage = "Número de telefone inválido")]
        [Display(Name = "Número de telefone")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Apelido do usuário")]
        public string? UserName { get; set; }

        [EmailAddress(ErrorMessage = "E-mail inválido")]
        [Display(Name = "Novo E-mail")]
        public string? NewEmail { get; set; }
    }
}
