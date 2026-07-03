using BottleDeliveryTracker.Backend.Data;
using BottleDeliveryTracker.Backend.DTOs;
using BottleDeliveryTracker.Backend.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<BottleContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=bottles.db"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BottleContext>();
    context.Database.Migrate();
}

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/deliveries", async (BottleContext context) =>
{
    var deliveries = await context.DeliveryRecords
        .OrderByDescending(r => r.Date)
        .ThenByDescending(r => r.CreatedAt)
        .Select(r => new
        {
            r.Id,
            Date = r.Date.ToString("yyyy-MM-dd"),
            r.Count,
            r.CreatedAt
        })
        .ToListAsync();

    return Results.Ok(deliveries);
});

app.MapPost("/api/deliveries", async (DeliveryCreateDto dto, BottleContext context) =>
{
    if (dto.Count <= 0)
    {
        return Results.BadRequest(new { message = "Count must be greater than zero." });
    }

    var delivery = new DeliveryRecord
    {
        Date = dto.Date.Date,
        Count = dto.Count
    };

    context.DeliveryRecords.Add(delivery);
    await context.SaveChangesAsync();

    return Results.Created($"/api/deliveries/{delivery.Id}", new
    {
        delivery.Id,
        Date = delivery.Date.ToString("yyyy-MM-dd"),
        delivery.Count,
        delivery.CreatedAt
    });
});

app.MapGet("/api/monthly-summary", async (BottleContext context) =>
{
    var summary = await context.DeliveryRecords
        .GroupBy(r => new { r.Date.Year, r.Date.Month })
        .Select(group => new MonthlySummaryDto
        {
            Year = group.Key.Year,
            Month = group.Key.Month,
            Total = group.Sum(r => r.Count)
        })
        .OrderByDescending(dto => dto.Year)
        .ThenByDescending(dto => dto.Month)
        .ToListAsync();

    return Results.Ok(summary);
});

app.MapGet("/api/monthly-summary/latest", async (BottleContext context) =>
{
    var mostRecent = await context.DeliveryRecords
        .OrderByDescending(r => r.Date)
        .FirstOrDefaultAsync();

    if (mostRecent == null)
    {
        return Results.Ok(new { message = "No entries yet." });
    }

    var monthSummary = await context.DeliveryRecords
        .Where(r => r.Date.Year == mostRecent.Date.Year && r.Date.Month == mostRecent.Date.Month)
        .SumAsync(r => r.Count);

    return Results.Ok(new
    {
        Year = mostRecent.Date.Year,
        Month = mostRecent.Date.Month,
        Total = monthSummary
    });
});

app.MapGet("/health", () => Results.Ok(new { status = "up" }));

app.Run();
