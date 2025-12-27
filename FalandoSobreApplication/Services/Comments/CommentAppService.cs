using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobreApplication.Interfaces.Comments;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace FalandoSobreApplication.Services.Comments;

public class CommentAppService : ICommentAppService
{
    private readonly ICommentRepository _commentRepository;
    private readonly AuthenticationStateProvider _auth;

    private readonly Dictionary<Guid, List<Comment>> _comments = new();
    private readonly Dictionary<Guid, string> _drafts = new();
    private readonly HashSet<Guid> _openReports = new();

    public IReadOnlyDictionary<Guid, List<Comment>> Comments => _comments;
    public IReadOnlyDictionary<Guid, string> Drafts => _drafts;
        

    public CommentAppService(
        ICommentRepository commentRepository,
        AuthenticationStateProvider auth)
    {
        _commentRepository = commentRepository;
        _auth = auth;
    }

    public bool IsOpen(Guid reportId) => _openReports.Contains(reportId);

    public void Toggle(Guid reportId)
    {
        if (!_openReports.Add(reportId))
            _openReports.Remove(reportId);
    }

    public async Task LoadAsync(IEnumerable<Report> reports)
    {
        _comments.Clear();
        _drafts.Clear();

        foreach (var report in reports)
        {
            var comments = await _commentRepository.GetByReportIdAsync(report.Id);

            _comments[report.Id] = comments
                .OrderByDescending(c => c.CommentDate)
                .ToList();

            _drafts[report.Id] = string.Empty;
        }
    }

    public async Task AddAsync(Guid reportId)
    {
        if (!_drafts.TryGetValue(reportId, out var text) || string.IsNullOrWhiteSpace(text))
            throw new InvalidOperationException("Comentário vazio.");

        var authState = await _auth.GetAuthenticationStateAsync();
        var user = authState.User;

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("Usuário não encontrado.");

        var comment = new Comment
        {
            CommentContent = text,
            CommentDate = DateTime.UtcNow,
            ReportId = reportId,
            UserName = user.Identity?.Name ?? "Anônimo",
            ApplicationUserId = userId
        };

        var created = await _commentRepository.AddAsync(comment);

        _comments[reportId].Insert(0, created);
        _drafts[reportId] = string.Empty;
    }

    public async Task DeleteAsync(Guid reportId, Guid commentId)
    {
        var authState = await _auth.GetAuthenticationStateAsync();
        var userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("Usuário não encontrado.");

        var comment = _comments[reportId].FirstOrDefault(c => c.Id == commentId)
            ?? throw new InvalidOperationException("Comentário não encontrado.");

        if (comment.ApplicationUserId != userId)
            throw new InvalidOperationException("Sem permissão.");

        await _commentRepository.DeleteAsync(commentId);
        _comments[reportId].Remove(comment);
    }

    public string GetDraft(Guid reportId)
    {
        return _drafts.TryGetValue(reportId, out var text)
            ? text
            : string.Empty;
    }

    public void SetDraft(Guid reportId, string value)
    {
        _drafts[reportId] = value;
    }

    public void ClearDraft(Guid reportId)
    {
        _drafts[reportId] = string.Empty;
    }
}
