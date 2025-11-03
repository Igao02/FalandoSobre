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
    protected HashSet<Guid> LikedReportIds { get; set; } = new();
    protected Dictionary<Guid, int> ReportLikeCounts { get; set; } = new();

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
            await LoadUserLikesAsync();
            await LoadLikeCountsAsync();
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

    protected async Task ToggleLikeAsync(Guid reportId)
    {
        isLoading = true;
        successMessage = string.Empty;
        errorMessage = string.Empty;

        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        try
        {
            var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                errorMessage = "Usuário não encontrado.";
                return;
            }

            // Verifica se o usuário já curtiu
            if (LikedReportIds.Contains(reportId))
            {
                // Remove o like
                var handler = LikeRepository as Handlers.LikeHandler;
                if (handler != null)
                {
                    var removed = await handler.RemoveLikeAsync(userIdString, reportId);
                    if (removed)
                    {
                        successMessage = "Like removido com sucesso!";
                        LikedReportIds.Remove(reportId);
                        if (ReportLikeCounts.ContainsKey(reportId))
                        {
                            ReportLikeCounts[reportId]--;
                        }
                    }
                }
            }
            else
            {
                // Adiciona o like
                ModelLike.ReportId = reportId;
                ModelLike.ApplicationUserId = userIdString;
                await LikeRepository.AddLikesAsync(ModelLike);
                successMessage = "Like adicionado com sucesso!";
                LikedReportIds.Add(reportId);
                if (ReportLikeCounts.ContainsKey(reportId))
                {
                    ReportLikeCounts[reportId]++;
                }
                else
                {
                    ReportLikeCounts[reportId] = 1;
                }
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Erro ao processar like: {ex.Message}";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    protected string? GetProfilePhoto(string userId) =>
        ModelUserInfo.FirstOrDefault(u => u.ApplicationUserId == userId)?.ProfilePhoto;

    private async Task LoadUserLikesAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userIdString))
        {
            var userLikes = await LikeRepository.GetLikesByUserIdAsync(userIdString);
            LikedReportIds = userLikes.Select(like => like.ReportId).ToHashSet();
        }
    }

    private async Task LoadLikeCountsAsync()
    {
        foreach (var report in Model)
        {
            var likes = await LikeRepository.GetLikesByReportIdAsync(report.Id);
            ReportLikeCounts[report.Id] = likes.Count();
        }
    }

    protected bool HasUserLiked(Guid reportId) => LikedReportIds.Contains(reportId);

    protected int GetLikeCount(Guid reportId) => ReportLikeCounts.ContainsKey(reportId) ? ReportLikeCounts[reportId] : 0;
}
