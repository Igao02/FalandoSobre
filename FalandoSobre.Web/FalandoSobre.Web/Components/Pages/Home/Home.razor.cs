using FalandoSobre.Domain.Dto.PagedRequest;
using FalandoSobre.Domain.Dto.PagedResponse;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FalandoSobre.Web.Components.Pages.Home;

public class HomePage : ComponentBase
{
    [Inject] public IReportRepository? ReportRepository { get; set; } = null!;
    [Inject] public NavigationManager? Navi { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;

    protected List<Report> Model { get; set; } = new();
    protected int CurrentPage { get; set; } = 1;
    protected int PageSize { get; set; } = 5;
    protected int TotalItems { get; set; }
    protected int TotalPages => (int)Math.Ceiling((double)TotalItems / (PageSize > 0 ? PageSize : 1));

    protected string successMessage = string.Empty;
    protected string errorMessage = string.Empty;
    protected bool isLoading = false;

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

    protected async Task LoadReportsAsync()
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

            // Recebe os dados paginados do backend
            var pagedResult = await ReportRepository!.GetListAsync(pagedRequest);

            // Atualiza os dados da lista de relatórios e o total de itens
            Model = pagedResult.Data;
            TotalItems = pagedResult.TotalItems;  // Total de itens vindos do backend
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