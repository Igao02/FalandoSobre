using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.Web.Components.Pages.Home.Dialogs;
using FalandoSobreApplication.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;

namespace FalandoSobre.Web.Components.Pages.Home;

public class HomePage : ComponentBase
{
    [Inject] public IReportAppService ReportAppService { get; set; } = null!;
    [Inject] public IReportRepository ReportRepository { get; set; } = null!;
    [Inject] public ILikeRepository LikeRepository { get; set; } = null!;
    [Inject] public ICommentRepository CommentRepository { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;


    protected List<Report> Model { get; set; } = new();
    protected List<UserInfo> ModelUserInfo { get; set; } = new();
    protected Like ModelLike { get; set; } = new();
    protected HashSet<Guid> LikedReportIds { get; set; } = new();
    protected Dictionary<Guid, int> ReportLikeCounts { get; set; } = new();

 
    protected Dictionary<Guid, List<Comment>> ReportComments { get; set; } = new();
    protected Dictionary<Guid, string> NewCommentText { get; set; } = new();
    protected HashSet<Guid> OpenCommentsForReports { get; set; } = new();

    protected string? CurrentUserId { get; set; }

    private int CurrentPage { get; set; } = 1;
    private int PageSize { get; set; } = 5;
    private int TotalItems { get; set; }
    protected int TotalPages => (int)Math.Ceiling((double)TotalItems / (PageSize > 0 ? PageSize : 1));

    public bool isLoading = false;
    public string successMessage = string.Empty;
    public string errorMessage = string.Empty;

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
        successMessage = errorMessage = string.Empty;

        try
        {
            (Model, TotalItems) = await ReportAppService.GetReportsAsync(CurrentPage, PageSize);
            ModelUserInfo = await ReportAppService.GetProfilePhotosAsync(Model);
            await LoadUserLikesAsync();
            await LoadLikeCountsAsync();
            await LoadCommentsAsync();
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

            if (LikedReportIds.Contains(reportId))
            {
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

    private async Task LoadCommentsAsync()
    {
        ReportComments.Clear();
        NewCommentText.Clear();

        foreach (var report in Model)
        {
            var comments = await CommentRepository.GetByReportIdAsync(report.Id);
            ReportComments[report.Id] = comments.OrderByDescending(c => c.CommentDate).ToList();
            NewCommentText[report.Id] = string.Empty;
        }
    }

    protected bool HasUserLiked(Guid reportId) => LikedReportIds.Contains(reportId);

    protected int GetLikeCount(Guid reportId) => ReportLikeCounts.ContainsKey(reportId) ? ReportLikeCounts[reportId] : 0;

    protected void ToggleComments(Guid reportId)
    {
        if (OpenCommentsForReports.Contains(reportId))
            OpenCommentsForReports.Remove(reportId);
        else
            OpenCommentsForReports.Add(reportId);
    }

    protected async Task AddCommentAsync(Guid reportId)
    {
        if (!NewCommentText.TryGetValue(reportId, out var text) || string.IsNullOrWhiteSpace(text))
        {
            errorMessage = "Digite um comentário antes de enviar.";
            return;
        }

        isLoading = true;
        successMessage = string.Empty;
        errorMessage = string.Empty;

        try
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            var userName = user.Identity?.Name ?? "Anônimo";
            var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                errorMessage = "Usuário não encontrado.";
                return;
            }

            var comment = new Comment
            {
                CommentContent = text,
                CommentDate = DateTime.UtcNow,
                ReportId = reportId,
                UserName = userName,
                ApplicationUserId = userIdString,
            };

            var created = await CommentRepository.AddAsync(comment);

            if (!ReportComments.ContainsKey(reportId))
                ReportComments[reportId] = new List<Comment>();

            ReportComments[reportId].Insert(0, created);
            NewCommentText[reportId] = string.Empty;

            successMessage = "Comentário enviado com sucesso!";
        }
        catch (Exception ex)
        {
            errorMessage = $"Erro ao enviar comentário: {ex.Message}";
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
        successMessage = string.Empty;
        errorMessage = string.Empty;

        try
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                errorMessage = "Usuário não encontrado.";
                return;
            }

            if (!ReportComments.TryGetValue(reportId, out var commentsForReport))
            {
                errorMessage = "Comentário não encontrado.";
                return;
            }

            var comment = commentsForReport.FirstOrDefault(c => c.Id == commentId);
            if (comment is null)
            {
                errorMessage = "Comentário não encontrado.";
                return;
            }

            if (comment.ApplicationUserId != CurrentUserId)
            {
                errorMessage = "Você não tem permissão para excluir este comentário.";
                return;
            }

            await CommentRepository.DeleteAsync(commentId);

            commentsForReport.Remove(comment);

            successMessage = "Comentário excluído com sucesso!";
        }
        catch (Exception ex)
        {
            errorMessage = $"Erro ao excluir comentário: {ex.Message}";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    protected async Task DeleteReportAsync(Guid reportId)
    {
        isLoading = true;
        successMessage = string.Empty;
        errorMessage = string.Empty;

        try
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                errorMessage = "Usuário não encontrado.";
                return;
            }

            var report = Model.FirstOrDefault(r => r.Id == reportId);
            if (report is null)
            {
                errorMessage = "Publicação não encontrada.";
                return;
            }

            if (report.ApplicationUserId != CurrentUserId)
            {
                errorMessage = "Você não tem permissão para excluir esta publicação.";
                return;
            }

            await ReportRepository.DeleteAsync(reportId);

            // Remove da lista atual sem precisar recarregar a página inteira
            Model.Remove(report);

            successMessage = "Publicação excluída com sucesso!";
        }
        catch (Exception ex)
        {
            errorMessage = $"Erro ao excluir publicação: {ex.Message}";
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
            errorMessage = "Você não tem permissão para editar esta publicação.";
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
        successMessage = string.Empty;
        errorMessage = string.Empty;

        try
        {
            var existing = Model.FirstOrDefault(r => r.Id == editedReport.Id);
            if (existing is null)
            {
                errorMessage = "Publicação não encontrada.";
                return;
            }

            if (string.IsNullOrEmpty(CurrentUserId) || existing.ApplicationUserId != CurrentUserId)
            {
                errorMessage = "Você não tem permissão para editar esta publicação.";
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

            successMessage = "Publicação atualizada com sucesso!";
        }
        catch (Exception ex)
        {
            errorMessage = $"Erro ao atualizar publicação: {ex.Message}";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

}
