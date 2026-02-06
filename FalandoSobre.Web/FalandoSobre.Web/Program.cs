using FalandoSobre.Domain.Repositories;
using FalandoSobre.Web.Components;
using FalandoSobre.Web.Components.Account;
using FalandoSobre.Web.Data;
using FalandoSobre.Web.Handlers;
using FalandoSobreApplication.Interfaces.Comments;
using FalandoSobreApplication.Interfaces.Feed;
using FalandoSobreApplication.Interfaces.Likes;
using FalandoSobreApplication.Interfaces.Reports;
using FalandoSobreApplication.Interfaces.SharedReports;
using FalandoSobreApplication.Services.Comments;
using FalandoSobreApplication.Services.Feed;
using FalandoSobreApplication.Services.Likes;
using FalandoSobreApplication.Services.Reports;
using FalandoSobreApplication.Services.SharedReports;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddHttpContextAccessor();
//builder.Services.AddAntiforgery();


builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
});

builder.Services.AddHttpClient();


builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7249");
});

builder.Services.AddTransient<IReportRepository, ReportHandler>();
builder.Services.AddTransient<IImageRepository, ImageHandler>();
builder.Services.AddTransient<IInstitutionRepository, InstitutionHandler>();
builder.Services.AddTransient<IUserInfoRepository, UserInfoHandler>();
builder.Services.AddTransient<ILikeRepository, LikeHandler>();
builder.Services.AddTransient<ICommentRepository, CommentHandler>();
builder.Services.AddTransient<ISharedReportRepository, SharedReportsHandler>();
builder.Services.AddTransient<IReportAppService, ReportAppService>();
builder.Services.AddScoped<ILikeAppService, LikeAppService>(); 
builder.Services.AddScoped<ICommentAppService, CommentAppService>();
builder.Services.AddScoped<ISharedReportsAppService, SharedReportsAppService>();
builder.Services.AddScoped<IFeedAppService, FeedAppService>();


builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddAuthorization();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    //.AddRoles<IdentityRole>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, EmailSenderService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseWebAssemblyDebugging();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(FalandoSobre.Web.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

await app.RunAsync();
