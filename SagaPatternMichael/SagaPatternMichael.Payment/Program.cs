using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SagaPatternMichael.Payment.Infrastructure.Core.Entities;
using SagaPatternMichael.Payment.Infrastructure.RabbitMQ;
using SagaPatternMichael.Payment.Infrastructure.RabbitMQ.Consumes;
using SagaPatternMichael.Payment.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<PaymentServiceContext>(x => x.UseSqlServer("Data Source=nguynminhvu;Initial Catalog=PaymentService;Integrated Security=True;Encrypt=False;Trust Server Certificate=True"));
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddSingleton<MessageConnection, MessageSupport>();

builder.Services.AddSingleton<MessageEventFactory>();

builder.Services.AddHostedService<MessageEventFactory>(x=>x.GetService<MessageEventFactory>()!);
builder.Services.AddHostedService<PaymentCompletedCommandConsume>();

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
