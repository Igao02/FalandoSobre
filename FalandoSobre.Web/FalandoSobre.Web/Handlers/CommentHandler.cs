using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobreApplication.UseCases.CommentUseCase.Create;
using FalandoSobreApplication.UseCases.CommentUseCase.ListByReportId;

namespace FalandoSobre.Web.Handlers;

public class CommentHandler(IHttpClientFactory httpClientFactory, ILogger<CommentHandler> logger)
    : ICommentRepository
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");

    public async Task<Comment> AddAsync(Comment comment)
    {
        var response = await _httpClient.PostAsJsonAsync("/create-comment", new
        {
            comment.ReportId,
            comment.CommentContent,
            comment.UserName,
            comment.ApplicationUserId 
        });

        if (response.IsSuccessStatusCode)
        {
            var createdResponse = await response.Content.ReadFromJsonAsync<CommentResponse>();
            if (createdResponse is null)
            {
                throw new ApplicationException("Resposta inválida ao criar comentário.");
            }

            var createdComment = new Comment(
                createdResponse.CommentContent,
                createdResponse.CommentDate,
                createdResponse.ReportId,
                createdResponse.UserName,
                createdResponse.ApplicationUserId
            )
            {
                Id = createdResponse.Id
            };

            logger.LogInformation("Comentário criado com sucesso!");
            return createdComment;
        }

        var error = await response.Content.ReadAsStringAsync();
        logger.LogError("Erro ao criar comentário: {Error}", error);
        throw new ApplicationException($"Erro ao criar comentário: {error}");
    }

    public async Task DeleteAsync(Guid id)
    {
        var url = $"/comments/{id}";
        var response = await _httpClient.DeleteAsync(url);

        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("Comentário {CommentId} removido com sucesso", id);
            return;
        }

        var error = await response.Content.ReadAsStringAsync();
        logger.LogError("Erro ao remover comentário {CommentId}: {Error}", id, error);
        throw new ApplicationException($"Erro ao remover comentário: {error}");
    }

    public Task<Comment> EditAsync(Comment comment)
    {
        // Ainda não utilizado no front; implementar quando existir endpoint correspondente
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Comment>> GetListAsync()
    {
        // Para evitar retorno gigante sem filtro, prefira usar GetByReportIdAsync no front.
        // Aqui devolvemos lista vazia por padrão.
        return Enumerable.Empty<Comment>();
    }

    public async Task<IEnumerable<Comment>> GetByReportIdAsync(Guid reportId)
    {
        var url = $"/comments/report/{reportId}";
        var response = await _httpClient.GetFromJsonAsync<ListCommentsByReportIdResponse>(url);

        if (response?.Comments == null)
            return Enumerable.Empty<Comment>();

        return response.Comments.Select(c => new Comment(
            c.CommentContent,
            c.CommentDate,
            c.ReportId,
            c.UserName,
            c.ApplicationUserId
        )
        {
            Id = c.Id
        });
    }

    public Task<Comment?> GetAsync(Guid id)
    {
        // Ainda não utilizado; implementar quando existir endpoint específico
        throw new NotImplementedException();
    }

    public Task<int> SumCommentAsync(Guid id)
    {
        // Ainda não utilizado; pode ser implementado com um endpoint dedicado se necessário
        throw new NotImplementedException();
    }
}
