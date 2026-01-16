using FalandoSobre.Domain.Entities;
using FalandoSobre.Domain.Repositories;
using FalandoSobreApplication.Interfaces.SharedReports;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace FalandoSobreApplication.Services.SharedReports;

public class SharedReportsAppService : ISharedReportsAppService
{
    private readonly ISharedReportRepository _sharedReportRepository;
    private readonly AuthenticationStateProvider _auth;

    public SharedReportsAppService(ISharedReportRepository sharedReportRepository, AuthenticationStateProvider auth)
    {
        _sharedReportRepository = sharedReportRepository;
        _auth = auth;
    }

    public async Task<SharedReport> AddAsync(Guid reportId)
    {
        var authState = await _auth.GetAuthenticationStateAsync();
        var user = authState.User;

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
           ?? throw new InvalidOperationException("Usuário não encontrado.");

        var sharedReport = new SharedReport
        {
            Actived = true,
            ApplicationUserId = userId,
            CreatedAt = DateTime.UtcNow,
            ReportId = reportId
        };

        var created = await _sharedReportRepository.Create(sharedReport);
        return created;
    }
}
