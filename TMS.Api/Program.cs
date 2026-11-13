using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TMS.Api.Exceptions;
using TMS.Application.Interfaces;
using TMS.Application.Services;
using TMS.Infrastructure.Data;
using TMS.Infrastructure.Repositories;
using TMS.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateLifetime = true,
             
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHashingService, PasswordHashingService>();
builder.Services.AddScoped<ITemporaryPasswordService, TemporaryPasswordService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IChangePasswordService, ChangePasswordService>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

builder.Services.AddHttpClient<IEmailService, EmailService>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["NotificationSettings:NotificationBaseUrl"]!
        );
});

builder.Services.AddHttpClient<IAccountValidationService, AccountValidationService>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["BankApi:BaseUrl"]!
        );
});
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IRefreshTokenApplicationService, RefreshTokenApplicationService>();
builder.Services.AddScoped<ILogoutService, LogoutService>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<ITransactionService, TransactionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
