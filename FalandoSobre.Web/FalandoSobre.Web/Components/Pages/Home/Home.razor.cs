using FalandoSobre.Domain.Dto.PagedRequest;
using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using Microsoft.AspNetCore.Components;

namespace FalandoSobre.Web.Components.Pages.Home;

public class HomePage : ComponentBase
{
    [Inject] public IReportRepository? ReportRepository { get; set; } = null!;
    [Inject] public IImageRepository? ImageRepository { get; set; } = null!;
    [Inject] public IUserInfoRepository? UserInfoRepository { get; set; } = null!;

    protected List<Report> Model { get; set; } = new();
    protected List<UserInfo> ModelUserInfo { get; set; } = new();
    private int CurrentPage { get; set; } = 1;
    private int PageSize { get; set; } = 5;
    private int TotalItems { get; set; }
    protected int TotalPages => (int)Math.Ceiling((double)TotalItems / (PageSize > 0 ? PageSize : 1));

    public string successMessage = string.Empty;
    public string errorMessage = string.Empty;
    public bool isLoading = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadReportsAsync();
        await GetProfilePhotos();
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
                        report.Images = new();
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

    protected async Task GetProfilePhotos()
    {
        isLoading = true;
        successMessage = string.Empty;
        errorMessage = string.Empty;

        try
        {

            foreach (var publications in Model)
            {
                Console.WriteLine($"Buscando foto de perfil para o usuário: {publications.ApplicationUserId}");
                var imageResult = await UserInfoRepository!.GetImageByUserId(publications.ApplicationUserId);
                if (imageResult is not null)
                {
                    var userInfo = new UserInfo
                    {
                        Id = imageResult.Id,
                        ProfilePhoto = imageResult.ProfilePhoto,
                        ApplicationUserId = imageResult.ApplicationUserId,
                        CreatedAt = imageResult.CreatedAt
                    };
                    ModelUserInfo.Add(userInfo);
                    Console.WriteLine($"Usuário {publications.ApplicationUserId} - Foto de perfil: {imageResult.ProfilePhoto}");
                }
                else
                {
                    ModelUserInfo.Add(new UserInfo
                    {
                        Id = Guid.Empty,
                        ProfilePhoto = string.Empty,
                        ApplicationUserId = publications.ApplicationUserId,
                        CreatedAt = DateTime.UtcNow
                    });
                    Console.WriteLine($"Usuário {publications.ApplicationUserId} - Foto de perfil não encontrada.");
                }
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Erro ao carregar as fotos de perfil: {ex.Message}";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    protected string? GetProfilePhoto(string userId)
    {
        var teste = ModelUserInfo.FirstOrDefault(u => u.ApplicationUserId == userId)?.ProfilePhoto;

        Console.WriteLine($"Buscando foto de perfil para o usuário: {userId} - Resultado: {teste}");

        return teste;
    }

}