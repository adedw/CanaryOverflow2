using System.Text.Json.Serialization;

using CanaryOverflow2.Domain.Common;
using CanaryOverflow2.Domain.Question.Models;
using CanaryOverflow2.Infrastructure;

using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;

using CanaryOverflow2.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ProblemDetailsExtensions.AddProblemDetails(builder.Services)
    .AddControllers()
    .AddProblemDetailsConventions()
    .AddJsonOptions(config =>
    {
        config.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        config.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEventStoreClient(
    "esdb://localhost:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000");
builder.Services.AddTransient<IAggregateRepository<Guid, Question>, AggregateRepository<Guid, Question>>();
builder.Services.AddTransient<IQuestionService, QuestionService>();

var app = builder.Build();

app.UseProblemDetails();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();    
}

app.UseAuthorization();

app.MapControllers();

app.Run();