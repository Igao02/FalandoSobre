using FalandoSobre.Api.Endpoints.Reports;
using FalandoSobre.Api.Extensions;
using FalandoSobre.Application.UseCases.ReportUseCase.CreateUseCase;
using FalandoSobre.Domain.Repositories;
using FalandoSobre.Infrastructure.Repositories;
using FalandoSobre.Web.Data;
using FalandoSobre.Web.Handlers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiWeb", policy =>
    {
        policy.WithOrigins("https://localhost:7188")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configuração do Banco de Dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configuração de Identity
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddSignInManager()
.AddRoles<IdentityRole>()
.AddDefaultTokenProviders()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddAntiforgery();


builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
});

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7249");
});

// Autenticação e Autorização
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies();

builder.Services.AddAuthorization();

// Serviços da Aplicação
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateReportCommand).Assembly));

//builder.Services.AddTransient<IReportRepository, ReportHandler>();
builder.Services.AddTransient<IReportRepository, ReportRepository>();
//builder.Services.AddTransient<IImageRepository, ImageRepository>();
//builder.Services.AddTransient<ICommentRepository, CommentRepository>();
//builder.Services.AddTransient<ILikeRepository, LikeRepository>();
//builder.Services.AddTransient<IInstitutionRepository, InstitutionRepository>();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, EmailSenderService>();

// Configuração de Swagger e API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registro de Endpoints
builder.Services.AddEndpoints(typeof(CreateReportEndpoint).Assembly);

builder.Services.AddAntiforgery();

var app = builder.Build();

app.UseCors("ApiWeb");

// Configuração do pipeline de requisição
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

// Mapeando a API
app.MapIdentityApi<ApplicationUser>();


// Mapear os endpoints definidos na classe CreateReportEndpoint
app.MapEndpoints();

await app.RunAsync();
