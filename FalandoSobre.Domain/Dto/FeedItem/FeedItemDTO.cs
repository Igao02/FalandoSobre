using FalandoSobre.Domain.Entities;

namespace FalandoSobre.Domain.Dto.FeedItem;

public class FeedItemDTO
{
    public Guid EventId { get; set; }          // Id do Report ou SharedReport
    public DateTime EventDate { get; set; }    // Data que ordena o feed

    public bool IsShared { get; set; }

    // Dados do compartilhamento
    public string? SharedByUserId { get; set; }
    public string? SharedByUserName { get; set; }
    public string? SharedByUserPhoto { get; set; }

    // Conteúdo principal
    public Report Report { get; set; } = null!;
}
