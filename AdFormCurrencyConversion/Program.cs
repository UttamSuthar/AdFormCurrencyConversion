using AdFormCurrencyConversion.Models;
using AdFormCurrencyConversion.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddTransient<ILiveRatesService, LiveRatesService>();
builder.Services.AddTransient<IExchangeRateService, ExchangeRateService>();

// read connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// register DbContext
builder.Services.AddDbContext<AdFormCurrencyContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddHostedService<CurrencyRatesSchedulerService>(); // by default singlton
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
