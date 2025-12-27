using FalandoSobre.Domain.Entities;

namespace FalandoSobreApplication.Interfaces.Comments;

public interface ICommentAppService
{
    IReadOnlyDictionary<Guid, List<Comment>> Comments { get; }
    IReadOnlyDictionary<Guid, string> Drafts { get; }
    bool IsOpen(Guid reportId);
    Task LoadAsync(IEnumerable<Report> reports);
    Task AddAsync(Guid reportId);
    Task DeleteAsync(Guid reportId, Guid commentId);
    void Toggle(Guid reportId);
    String GetDraft(Guid reportId);
    void SetDraft(Guid reportId, string text);
    void ClearDraft(Guid reportId);
}
