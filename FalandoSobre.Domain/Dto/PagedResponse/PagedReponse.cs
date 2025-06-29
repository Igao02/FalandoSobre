namespace FalandoSobre.Domain.Dto.PagedResponse;

public class PagedResponse<T>
{
    public T Data { get; set; }
    public int TotalItems { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public PagedResponse(T data, int totalItems, int pageNumber, int pageSize)
    {
        Data = data;
        TotalItems = totalItems;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}

