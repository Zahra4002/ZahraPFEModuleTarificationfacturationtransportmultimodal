using API.Extension;
using Application.Features.QuoteFeature.Commands;
using Application.Interfaces;
using Application.Mappings;
using Application.Security;
using Application.Services;
using Infrastructure.Persistance.Repositories;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistance.Data;
using Persistance.Repositories;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IConfiguration configuration = builder.Configuration;

builder.Services.ConfigureContext(configuration);
builder.Services.AddControllers();
builder.Services.RetryExtension(configuration);
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();
builder.Services.AddCors(options => options.AddPolicy("cors", builder =>
{
    builder
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials();
}));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

builder.Services.ConfigureSwagger();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<AutoMapperProfiles>();
});

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(UpdateQuoteCommandHandler).Assembly);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "defaultSecretKey12345678901234567890")),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

// ==================== REPOSITORIES ====================
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IMerchandiseTypeRepository, MerchandiseTypeRepository>();
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();

// ==================== EXCHANGERATE.HOST SERVICE ====================
builder.Services.AddHttpClient<IExchangeRateHostService, ExchangeRateHostService>();
builder.Services.AddScoped<IExchangeRateHostService, ExchangeRateHostService>();
// ==================== AUTRES REPOSITORIES ====================
// builder.Services.AddScoped<IClientRepository, ClientRepository>();
// builder.Services.AddScoped<IContractRepository, ContractRepository>();

// ===========================================================================

builder.Services.AddAuthorization();
builder.Services.AddDbContext<CleanArchitecturContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

var app = builder.Build();
app.UseRouting();
app.UseDeveloperExceptionPage();
app.UseCors("cors");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();