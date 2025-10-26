using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobreApplication.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace FalandoSobre.Web.Components.Pages.Home;

public class HomePage : ComponentBase
{
    [Inject] public IReportAppService ReportAppService { get; set; } = null!;
    [Inject] public ILikeRepository LikeRepository { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;


    protected List<Report> Model { get; set; } = new();
    protected List<UserInfo> ModelUserInfo { get; set; } = new();
    protected Like ModelLike { get; set; } = new();

    private int CurrentPage { get; set; } = 1;
    private int PageSize { get; set; } = 5;
    private int TotalItems { get; set; }
    protected int TotalPages => (int)Math.Ceiling((double)TotalItems / (PageSize > 0 ? PageSize : 1));

    public bool isLoading = false;
    public string successMessage = string.Empty;
    public string errorMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    protected async Task OnPageChanged(int page)
    {
        if (CurrentPage != page)
        {
            CurrentPage = page;
            await LoadDataAsync();
        }
    }

    private async Task LoadDataAsync()
    {
        isLoading = true;
        successMessage = errorMessage = string.Empty;

        try
        {
            (Model, TotalItems) = await ReportAppService.GetReportsAsync(CurrentPage, PageSize);
            ModelUserInfo = await ReportAppService.GetProfilePhotosAsync(Model);
        }
        catch (Exception ex)
        {
            errorMessage = $"Erro ao carregar os dados: {ex.Message}";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    protected async Task CreateLikeAsync(Guid reportId)
    {
        isLoading = true;
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

            ModelLike.ReportId = reportId;
            ModelLike.ApplicationUserId = userIdString;
            await LikeRepository.AddLikesAsync(ModelLike);
            successMessage = "Like adicionado com sucesso!";
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            errorMessage = $"Erro ao adicionar like: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }

    protected string? GetProfilePhoto(string userId) =>
        ModelUserInfo.FirstOrDefault(u => u.ApplicationUserId == userId)?.ProfilePhoto;
}
