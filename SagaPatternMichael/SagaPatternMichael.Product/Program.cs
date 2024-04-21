using Microsoft.EntityFrameworkCore;
using SagaPatternMichael.Product.Infrastructure.Core.Entities;
using SagaPatternMichael.Product.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ShopPartTimeContext>(x => x.UseSqlServer("Data Source=nguynminhvu;Initial Catalog=ShopPartTime;Integrated Security=True;Encrypt=False;Trust Server Certificate=True"));
builder.Services.AddScoped<IProductService, ProductService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
