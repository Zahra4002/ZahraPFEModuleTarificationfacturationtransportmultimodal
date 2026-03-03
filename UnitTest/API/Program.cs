using API.Extension;
using Application.Interfaces;
using Application.Interfaces.Security;
using Application.Mappings;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistance.Data;
using Persistance.Repositories;
using System.Text;

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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.ConfigureSwagger();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));


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
builder.Services.AddAuthorization();


builder.Services.AddScoped<Application.Interfaces.IUserRepository, Persistance.Repositories.UserRepository>();

builder.Services.AddDbContext<CleanArchitecturContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITariffGridRepository, TariffGridRepository>();


builder.Services.AddScoped<ISurchargeRepository, SurchargeRepository>();
var app = builder.Build();
app.UseRouting();
// Configure the HTTP request pipeline.
app.UseCors("cors");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();