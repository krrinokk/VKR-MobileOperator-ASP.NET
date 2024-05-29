using OperatorMO_ASPNET.DAL.Models;
using OperatorMO_ASPNET.DAL.Repository;

using BLL.Interfaces;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using System.Text.Json.Serialization;

using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OperatorMO_ASPNET.DAL;
using Hangfire;
using Hangfire.SqlServer; 

// Создание объекта builder для построения веб-приложения
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<OperatorContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Очистка провайдеров логирования и добавление провайдера Console
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


//// Добавление политики CORS, разрешающей запросы только с сайта http://localhost:8084
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:8000")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

//builder.Services.AddIdentity<User, IdentityRole>()
//    .AddEntityFrameworkStores<OperatorContext>()
//    .AddDefaultTokenProviders();

// Добавление сервисов в контейнер
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddIdentity<User, IdentityRole>()
//.AddEntityFrameworkStores<OperatorContext>();

builder.Services.AddControllers().AddJsonOptions(x =>
x.JsonSerializerOptions.ReferenceHandler =
ReferenceHandler.IgnoreCycles);
builder.Services.AddScoped(typeof(UnitOfWork));
builder.Services.AddScoped(typeof(DbDataOperations));
builder.Services.AddTransient<IDbRepos, UnitOfWork>();
builder.Services.AddTransient<IDbCrud, DbDataOperations>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
        options =>
        {
            options.LoginPath = new PathString("/api/Account/login");
            options.AccessDeniedPath = new PathString("/api/Account/check-auth");
        });
// Конфигурирование IdentityOptions
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});

// Конфигурирование ApplicationCookieOptions
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "OperatorApp";
    options.LoginPath = "/";
    options.AccessDeniedPath = "/";
    options.LogoutPath = "/";
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
});

// Построение объекта приложения
var app = builder.Build();
Log.Information("Application started");

// Заполнение базы данных начальными данными
//try
//{
//    using (var scope = app.Services.CreateScope())
//    {
//        var OperatorContext =
//        scope.ServiceProvider.GetRequiredService<OperatorContext>();
//        //await OperatorContextSeed.SeedAsync(OperatorContext);

//        //await IdentitySeed.CreateUserRoles(scope.ServiceProvider);
//    }
//}
//catch (Exception ex)
//{
//    Log.Error(ex, "An error occurred while seeding the database.");
//}

// Конфигурирование конвейера обработки HTTP-запросов
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}






app.UseHttpsRedirection();
// Разрешение запросов CORS
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
Log.Information("Application stopped");