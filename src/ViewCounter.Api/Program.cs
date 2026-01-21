using Microsoft.EntityFrameworkCore;
using ViewCounter.Application.Abstractions.Repositories;
using ViewCounter.Application.Abstractions.Services;
using ViewCounter.Application.Views.Handlers;
using ViewCounter.Domain.Interfaces;
using ViewCounter.Infrastructure.Persistence;
using ViewCounter.Infrastructure.Policies;
using ViewCounter.Infrastructure.Repositories;
using ViewCounter.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ViewCounterDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IViewEventRepository, ViewEventRepository>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<IViewDeduplicationPolicy, DatabaseDeduplicationPolicy>();
builder.Services.AddScoped<IViewRateLimiterService, ViewRateLimiterService>();

// DI for Application Handlers
builder.Services.AddScoped<RegisterViewHandler>();

builder.Services.AddMemoryCache();

builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();