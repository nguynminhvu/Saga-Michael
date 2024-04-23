using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SagaPatternMichael.Product.Infrastructure;
using SagaPatternMichael.Product.Infrastructure.Core.Entities;
using SagaPatternMichael.Product.Infrastructure.RabbitMQ;
using SagaPatternMichael.Product.Infrastructure.RabbitMQ.Consumes;
using SagaPatternMichael.Product.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ShopPartTimeContext>(x => x.UseSqlServer("Data Source=nguynminhvu;Initial Catalog=ShopPartTime;Integrated Security=True;Encrypt=False;Trust Server Certificate=True"));
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddSingleton<MessageEventFactory>();
builder.Services.AddSingleton<EventFactory>();

builder.Services.AddSingleton<MessageConnection, MessageSupport>();

builder.Services.AddHostedService<MessageEventFactory>(x=>x.GetService<MessageEventFactory>()!);
builder.Services.AddHostedService<EventFactory>(x=>x.GetService<EventFactory>()!);

builder.Services.AddHostedService<InventoryCompletedCommandConsume>();
builder.Services.AddHostedService<InventoryErrorCommandConsume>();

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
