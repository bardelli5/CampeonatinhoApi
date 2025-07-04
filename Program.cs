using System;
using CampeonatinhoApp.Context;
using CampeonatinhoApp.Interfaces;
using CampeonatinhoApp.Models;
using CampeonatinhoApp.Repositories;
using CampeonatinhoApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();

builder.Services.AddDbContext<CampeonatinhoDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<CampeonatinhoDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<FootballApiRequestService>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();
builder.Services.AddScoped<IClubRepository, ClubRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<EmailSenderService>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var apiService = services.GetRequiredService<FootballApiRequestService>();
//    var dbContextService = services.GetRequiredService<CampeonatinhoDbContext>();

//    var jsonData = apiService.GetApiDataClubs();
//}

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var apiService = services.GetRequiredService<FootballApiRequestService>();
//    var dbContextService = services.GetRequiredService<CampeonatinhoDbContext>();

//    var jsonData = apiService.GetApiDataLeagues();
//}

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var apiService = services.GetRequiredService<FootballApiRequestService>();
//    var dbContextService = services.GetRequiredService<CampeonatinhoDbContext>();

//    var jsonData = apiService.GetApiDataCountries();
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
