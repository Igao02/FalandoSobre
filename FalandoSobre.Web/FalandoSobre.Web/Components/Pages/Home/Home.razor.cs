using FalandoSobre.Domain.Entities;
using FalandoSobreApplication.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FalandoSobre.Web.Components.Pages.Home;

public class HomePage : ComponentBase
{
    [Inject] public IReportAppService ReportAppService { get; set; } = null!;

    protected List<Report> Model { get; set; } = new();
    protected List<UserInfo> ModelUserInfo { get; set; } = new();

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
            Console.WriteLine("Model aqui igão: ", Model);
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


    protected string? GetProfilePhoto(string userId) =>
        ModelUserInfo.FirstOrDefault(u => u.ApplicationUserId == userId)?.ProfilePhoto;
}
