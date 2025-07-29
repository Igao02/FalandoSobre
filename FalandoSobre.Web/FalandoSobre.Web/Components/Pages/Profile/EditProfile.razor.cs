using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.Web.Components.Account;
using FalandoSobre.Web.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace FalandoSobre.Web.Components.Pages.Profile;

public partial class EditProfilePage : ComponentBase
{
    protected ApplicationUser user = default!;
    protected string? username;
    protected string? phoneNumber;
    protected string? email;
    protected bool isEmailConfirmed;
    protected string? profilePhoto;
    protected string? message;
    protected bool isLoading = false;
    protected List<IBrowserFile> selectedFiles = new List<IBrowserFile>();
    protected List<string> imagePreviewUrls = new List<string>();

    protected InputModel Model { get; set; } = new();

    [Inject] IdentityUserAccessor UserAccessor { get; set; } = default!;
    [Inject] protected UserManager<ApplicationUser> UserManager { get; set; } = default!;
    [Inject] protected SignInManager<ApplicationUser> SignInManager { get; set; } = default!;
    [Inject] protected IHttpContextAccessor HttpContextAccessor { get; set; } = default!;
    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject] protected IEmailSender<ApplicationUser> EmailSender { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;
    [Inject] protected IUserInfoRepository? UserInfoRepository { get; set; } = null!;
    [Inject] protected ILogger<EditProfilePage>? Logger { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var httpContext = HttpContextAccessor.HttpContext;
            user = await UserAccessor.GetRequiredUserAsync(httpContext!);

            username = await UserManager.GetUserNameAsync(user);
            phoneNumber = await UserManager.GetPhoneNumberAsync(user);
            email = await UserManager.GetEmailAsync(user);
            isEmailConfirmed = await UserManager.IsEmailConfirmedAsync(user);

            Model.PhoneNumber ??= phoneNumber;
            Model.UserName ??= username;
            Model.NewEmail ??= email;
            Model.ProfilePhoto ??= profilePhoto;
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Erro ao carregar perfil: {ex.Message}", Severity.Error);
        }
    }

    protected async Task OnValidSubmitAsync()
    {
        isLoading = true;
        message = null;

        try
        {
            if (Model.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await UserManager.SetPhoneNumberAsync(user, Model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    Snackbar.Add("Erro ao atualizar número de telefone", Severity.Error);
                    return;
                }
                phoneNumber = Model.PhoneNumber;
                Snackbar.Add("Número de telefone atualizado com sucesso!", Severity.Success);
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

                Snackbar.Add("Link de confirmação para alteração de email enviado. Por favor verifique seu email.", Severity.Info);
                message = "Confirmation link to change email sent. Please check your email.";
            }

            if (Model.UserName is not null && Model.UserName != username)
            {
                if (!Regex.IsMatch(Model.UserName, @"^[a-zA-Z0-9\s_-]+$"))
                {
                    Snackbar.Add("Apelido inválido. Use apenas letras, números, espaços, hífens e sublinhados.", Severity.Warning);
                    return;
                }

                var setUserNameResult = await UserManager.SetUserNameAsync(user, Model.UserName);
                if (!setUserNameResult.Succeeded)
                {
                    Snackbar.Add("Erro ao atualizar apelido", Severity.Error);
                    return;
                }

                username = Model.UserName;
                Snackbar.Add("Apelido atualizado com sucesso!", Severity.Success);

            }

            if (selectedFiles.Count > 0)
            {
                var file = selectedFiles[0];
                var maxFileSize = 10 * 1024 * 1024;
                using var stream = file.OpenReadStream(maxAllowedSize: maxFileSize);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                await UserInfoRepository!.AddAsync(new UserInfo
                {
                    ApplicationUserId = user.Id,
                    ProfilePhotoBytes = fileBytes
                });

                Snackbar.Add("Imagem de perfil enviada com sucesso!", Severity.Success);
                await Task.Delay(2500);
            }

            if (Model.UserName != username || Model.PhoneNumber != phoneNumber || Model.ProfilePhoto != profilePhoto)
            {
                await JSRuntime.InvokeVoidAsync("window.location.reload");
            }

        }
        catch (Exception ex)
        {
            Snackbar.Add($"Erro ao atualizar perfil: {ex.Message}", Severity.Error);
        }
        finally
        {
            isLoading = false;
        }
    }

    protected async Task OnSendEmailVerificationAsync()
    {
        if (email is null)
        {
            return;
        }

        try
        {
            isLoading = true;

            var userId = await UserManager.GetUserIdAsync(user);
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = NavigationManager.GetUriWithQueryParameters(
                NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
                new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code });

            await EmailSender.SendConfirmationLinkAsync(user, email, callbackUrl);

            Snackbar.Add("Email de verificação enviado. Por favor verifique seu email.", Severity.Info);
            message = "Verification email sent. Please check your email.";
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Erro ao enviar email de verificação: {ex.Message}", Severity.Error);
        }
        finally
        {
            isLoading = false;
        }
    }

     protected async Task HandleSelectedFiles(InputFileChangeEventArgs e)
    {
        const long MaxFileSize = 10 * 1024 * 1024;
        const int BufferSize = 8192;
        selectedFiles = new List<IBrowserFile>();
        imagePreviewUrls.Clear();

        try
        {
            Logger?.LogInformation("Selecionando arquivos...");

            foreach (var file in e.GetMultipleFiles())
            {
                if (file.Size > MaxFileSize)
                {
                    Logger?.LogInformation($"O arquivo {file.Name} excede o tamanho permitido de 10 MB.");
                    continue;
                }

                selectedFiles.Add(file);

                // Gera um nome único para o arquivo
                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.Name)}";
                Model.ProfilePhoto = uniqueFileName;

                // Pré-visualização (opcional)
                using var stream = file.OpenReadStream(maxAllowedSize: MaxFileSize);
                using var memoryStream = new MemoryStream();
                var buffer = new byte[BufferSize];
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                }
                var imageUrl = $"data:{file.ContentType};base64,{Convert.ToBase64String(memoryStream.ToArray())}";
                imagePreviewUrls.Add(imageUrl);

                break; // Só aceita uma imagem
            }

            Logger?.LogInformation("Arquivos processados com sucesso.");
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Logger?.LogInformation($"Erro ao selecionar arquivos: {ex.Message}");
        }
    }

    public class InputModel
    {
        [Phone(ErrorMessage = "Número de telefone inválido")]
        [Display(Name = "Número de telefone")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "O apelido é obrigatório")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "O apelido deve ter entre 3 e 20 caracteres")]
        [Display(Name = "Apelido do usuário")]
        public string? UserName { get; set; }

        [EmailAddress(ErrorMessage = "E-mail inválido")]
        [Display(Name = "Novo E-mail")]
        public string? NewEmail { get; set; }
        public string? ProfilePhoto { get; set; }

    }
}