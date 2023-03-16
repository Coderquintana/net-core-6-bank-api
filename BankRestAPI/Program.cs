using BankRestAPI.Data;
using BankRestAPI.Models;
using BankRestAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//builder.Services.AddControllers(
//    options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
// Add database context
builder.Services.AddDbContext<BankDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("BankAPIConnectionString")));

// Inject service dependency 
builder.Services.AddScoped<BankService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<TransferService>();
builder.Services.AddScoped<CustomerService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
