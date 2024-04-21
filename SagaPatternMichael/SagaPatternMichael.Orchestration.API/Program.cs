using Microsoft.EntityFrameworkCore;
using SagaPatternMichael.Orchestration.Data;
using SagaPatternMichael.Orchestration.OrchestrationConsume;
using SagaPatternMichael.Orchestration.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<ErrorEventHandlerSg>();
builder.Services.AddHostedService<EventHandlerSg>();
builder.Services.AddHostedService<EventFactory>(x => x.GetService<EventFactory>()!);
builder.Services.AddHostedService<EventErrorFactory>(x => x.GetService<EventErrorFactory>()!);
builder.Services.AddDbContext<OrchestrationMichaelContext>(x => x.UseSqlServer("Data Source=nguynminhvu;Initial Catalog=OrchestrationMichael;Integrated Security=True;Encrypt=False;Trust Server Certificate=True"));
builder.Services.AddScoped<IOrchestrationService, OrchestrationService>();
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
