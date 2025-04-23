using System;
using CampeonatinhoApp.Context;
using CampeonatinhoApp.Interfaces;
using CampeonatinhoApp.Repositories;
using CampeonatinhoApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CampeonatinhoDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<FootballApiRequestService>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var apiService = services.GetRequiredService<FootballApiRequestService>();
    var dbContextService = services.GetRequiredService<CampeonatinhoDbContext>();

    var jsonData = apiService.GetApiDataLeaguesAsync();
}

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
