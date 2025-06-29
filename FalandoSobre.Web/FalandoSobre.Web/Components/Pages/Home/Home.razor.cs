using FalandoSobre.Domain.Dto.PagedRequest;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FalandoSobre.Web.Components.Pages.Home;

public class HomePage : ComponentBase
{
    [Inject] public IReportRepository? ReportRepository { get; set; } = null!;
    [Inject] public IImageRepository? ImageRepository { get; set; } = null!;
    [Inject] public NavigationManager? Navi { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;

    protected List<Report> Model { get; set; } = new();
    private List<Image> Images { get; set; } = new();
    private int CurrentPage { get; set; } = 1;
    private int PageSize { get; set; } = 5;
    private int TotalItems { get; set; }
    protected int TotalPages => (int)Math.Ceiling((double)TotalItems / (PageSize > 0 ? PageSize : 1));

    private string successMessage = string.Empty;
    private string errorMessage = string.Empty;
    private bool isLoading = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadReportsAsync();
    }

    protected async Task OnPageChanged(int page)
    {
        if (CurrentPage != page)
        {
            CurrentPage = page;
            await LoadReportsAsync();
        }
    }

    private async Task LoadReportsAsync()
    {
        isLoading = true;
        successMessage = string.Empty;
        errorMessage = string.Empty;

        try
        {
            var pagedRequest = new PagedRequest
            {
                Page = CurrentPage,
                PageSize = PageSize
            };

            var pagedResult = await ReportRepository!.GetListAsync(pagedRequest);
            Model = pagedResult.Data;
            TotalItems = pagedResult.TotalItems;

            if (Model.Count > 0)
            {
                foreach (var report in Model)
                {
                    var imageResult = await ImageRepository!.GetImageByReportId(report.Id);

                    if (imageResult is not null)
                    {
                        var (id, imageUrl, reportId) = imageResult.Value;

                        report.Images = new List<Image>
                        {
                            new()
                            {
                                Id = id,
                                ImageUrl = imageUrl,
                                ReportId = reportId ?? Guid.Empty
                            }
                        };
                    }
                    else
                    {
                        report.Images = new(); // Para evitar null no front
                    }
                }
            }
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
}