using FalandoSobre.Domain.Dto.FeedItem;
using FalandoSobre.Domain.Dto.PagedRequest;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.Web.Components.Pages.Home.Dialogs.EditReport;
using FalandoSobreApplication.Interfaces.Comments;
using FalandoSobreApplication.Interfaces.Feed;
using FalandoSobreApplication.Interfaces.Likes;
using FalandoSobreApplication.Interfaces.Reports;
using FalandoSobreApplication.Interfaces.SharedReports;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;

namespace FalandoSobre.Web.Components.Pages.Home;

public class HomePage : ComponentBase
{
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;
    [Inject] public IReportAppService ReportAppService { get; set; } = null!;
    [Inject] public ILikeAppService LikeAppService { get; set; } = null!;
    [Inject] public ICommentAppService CommentAppService { get; set; } = null!;
    [Inject] public ISharedReportsAppService SharedReportsAppService { get; set; } = null!;
    [Inject] public IFeedAppService FeedAppService { get; set; } = null!;
    [Inject] public IReportRepository ReportRepository { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = default!;


    protected List<Report> Model { get; set; } = new()!;
    protected List<UserInfo> ModelUserInfo { get; set; } = new()!;
    protected HashSet<Guid> LikedReportIds { get; set; } = new()!;
    protected Dictionary<Guid, int> ReportLikeCounts { get; set; } = new()!;
    protected List<FeedItemDTO> Feed { get; set; } = new();


    protected string? CurrentUserId { get; set; }
    protected int CurrentPage { get; set; } = 1;
    private int PageSize { get; set; } = 5;
    private int TotalItems { get; set; }
    protected int TotalPages => (int)Math.Ceiling((double)TotalItems / (PageSize > 0 ? PageSize : 1));

    public bool isLoading = false;
    protected string? UserName;
    protected string? UserId;

    protected override async Task OnInitializedAsync()
    {
        await LoadCurrentUserAsync();
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

    private async Task LoadCurrentUserAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        CurrentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    private async Task LoadDataAsync()
    {
        isLoading = true;

        Console.WriteLine("Iniciando LoadDataAsync...");

        try
        {
            //(Model, TotalItems) = await ReportAppService.GetReportsAsync(CurrentPage, PageSize);
            var feedResponse = await FeedAppService.GetFeedAsync(new PagedRequest
            {
                Page = CurrentPage,
                PageSize = PageSize
            });

            Feed = feedResponse.Data;
            TotalItems = feedResponse.TotalItems;

            Console.WriteLine("Cheguei até aqui???");

            Model = Feed
            .Select(f => f.Report)
            .GroupBy(r => r.Id)
            .Select(g => g.First())
            .ToList();

            Console.WriteLine("Cheguei até aqui 2???");


            ModelUserInfo = await ReportAppService.GetProfilePhotosAsync(Model);
            await LoadUserLikesAsync();
            await LoadLikeCountsAsync();
            await CommentAppService.LoadAsync(Model);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Erro ao carregar os dados: {ex.Message}", Severity.Error);
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

        if (string.IsNullOrEmpty(CurrentUserId))
        {
            Snackbar.Add("Usuário não encontrado.", Severity.Error);
            return;
        }

        try
        {
            var alreadyLiked = LikedReportIds.Contains(reportId);

            var success = await LikeAppService.ToggleLikeAsync(
                CurrentUserId,
                reportId,
                alreadyLiked
            );

            if (!success)
            {
                Snackbar.Add("Erro ao processar like.", Severity.Error);
                return;
            }

            if (alreadyLiked)
            {
                LikedReportIds.Remove(reportId);
                ReportLikeCounts[reportId]--;
            }
            else
            {
                LikedReportIds.Add(reportId);
                ReportLikeCounts[reportId] =
                    ReportLikeCounts.TryGetValue(reportId, out var count)
                        ? count + 1
                        : 1;
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Erro ao processar like: {ex.Message}", Severity.Error);
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
        if (string.IsNullOrEmpty(CurrentUserId))
            return;

        var likedReports = await LikeAppService.GetLikedReportsByUserAsync(CurrentUserId);
        LikedReportIds = likedReports.ToHashSet();
    }


    private async Task LoadLikeCountsAsync()
    {
        foreach (var report in Model)
        {
            ReportLikeCounts[report.Id] =
                await LikeAppService.GetLikeCountAsync(report.Id);
        }
    }

    protected bool HasUserLiked(Guid reportId) => LikedReportIds.Contains(reportId);

    protected int GetLikeCount(Guid reportId) => ReportLikeCounts.ContainsKey(reportId) ? ReportLikeCounts[reportId] : 0;

    protected void ToggleComments(Guid reportId)
        => CommentAppService.Toggle(reportId);

    protected async Task AddCommentAsync(Guid reportId)
    {
        isLoading = true;
        try
        {
            await CommentAppService.AddAsync(reportId);
            Snackbar.Add("Comentário adicionado!", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Não foi possível adicionar o comentário: {ex.Message}", Severity.Error);
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    protected async Task DeleteCommentAsync(Guid reportId, Guid commentId)
    {
        isLoading = true;
        try
        {
            await CommentAppService.DeleteAsync(reportId, commentId);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    protected async Task DeleteReportAsync(Guid reportId)
    {
        try
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                Snackbar.Add("Usuário não encontrado.", Severity.Error);
                return;
            }

            var report = Model.FirstOrDefault(r => r.Id == reportId);
            if (report is null)
            {
                Snackbar.Add("Publicação não encontrada.", Severity.Error);
                return;
            }

            if (report.ApplicationUserId != CurrentUserId)
            {
                Snackbar.Add("Você não tem permissão para excluir esta publicação.", Severity.Error);
                return;
            }

            await ReportRepository.DeleteAsync(reportId);
            Model.Remove(report);

            Snackbar.Add("Publicação excluída com sucesso!", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Erro ao excluir publicação: {ex.Message}", Severity.Error);
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    protected async Task OpenEditReportDialogAsync(Report report)
    {
        if (string.IsNullOrEmpty(CurrentUserId) || report.ApplicationUserId != CurrentUserId)
        {
            Snackbar.Add("Você não tem permissão para editar esta publicação.", Severity.Error);
            StateHasChanged();
            return;
        }

        var parameters = new DialogParameters
        {
            { nameof(EditReportDialog.Report), report }
        };

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true
        };

        var result = await DialogService.ShowAsync<EditReportDialog>(
            "Editar publicação",
            parameters,
            options
        );

        if (result != null && result.Result != null)
        {
            var dialogResult = await result.Result;
            if (dialogResult != null && dialogResult.Data is Report editedReport)
            {
                await SaveEditedReportAsync(editedReport);
            }
        }
    }


    private async Task SaveEditedReportAsync(Report editedReport)
    {
        isLoading = true;
        try
        {
            var existing = Model.FirstOrDefault(r => r.Id == editedReport.Id);
            if (existing is null)
            {
                Snackbar.Add("Publicação não encontrada.", Severity.Error);
                return;
            }

            if (string.IsNullOrEmpty(CurrentUserId) || existing.ApplicationUserId != CurrentUserId)
            {
                Snackbar.Add("Você não tem permissão para editar esta publicação.", Severity.Error);
                return;
            }

            editedReport.ApplicationUserId = existing.ApplicationUserId;
            editedReport.UserName = existing.UserName;
            editedReport.IsEvent = existing.IsEvent;
            editedReport.Actived = existing.Actived;
            editedReport.ReportsDate = existing.ReportsDate;

            var updated = await ReportRepository.EditAsync(editedReport);

            // Atualiza o item em memória
            existing.ReportName = updated.ReportName;
            existing.TypeReport = updated.TypeReport;
            existing.ReportDescription = updated.ReportDescription;

            Snackbar.Add("Publicação atualizada com sucesso!", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Erro ao atualizar publicação: {ex.Message}", Severity.Error);
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    protected async Task CreateSharedReports(Guid reportId)
    {
        isLoading = true;
        try
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                UserName = user.Identity.Name;
                UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                await SharedReportsAppService.AddAsync(reportId, UserName!);
                Snackbar.Add("Publicação compartilhada com sucesso!", Severity.Success);
            }
        }
        catch
        {
            Snackbar.Add("Erro ao compartilhar publicação.", Severity.Error);
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    // Adicione este método na sua classe HomePage
    protected int GetShareCount(Guid reportId)
    {
        // Implemente a lógica para contar quantas vezes o relatório foi compartilhado
        // Exemplo: return Feed.Count(x => x.Report.Id == reportId && x.IsShared);
        return 0; // Substitua pela implementação real
    }

    // Método para remover compartilhamento
    protected async Task RemoveSharedReportAsync(Guid sharedReportId)
    {
        // Implemente a lógica para remover o compartilhamento
        // await _sharedReportService.RemoveSharedReportAsync(sharedReportId);
        // RefreshFeed(); // Atualize o feed após remover
    }

}
