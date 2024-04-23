using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SagaPatternMichael.Order.Core.Entities;
using SagaPatternMichael.Order.RabbitMQ;
using SagaPatternMichael.Order.RabbitMQ.Consumes;
using SagaPatternMichael.Order.Services;
using SagaPatternMichael.Order.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OrderServiceContext>(x => x.UseSqlServer("Data Source=nguynminhvu;Initial Catalog=OrderService;Integrated Security=True;Encrypt=False;Trust Server Certificate=True"));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddSingleton<MessageConnection, MessageSupport>();
builder.Services.AddSingleton<MessageEventFactory>();

builder.Services.AddHostedService<MessageEventFactory>(x=>x.GetService<MessageEventFactory>()!);
builder.Services.AddHostedService<OrderCompletedCommandConsume>();
builder.Services.AddHostedService<OrderErrorCommandConsume>();

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
