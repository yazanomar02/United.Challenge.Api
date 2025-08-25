using Microsoft.AspNetCore.Mvc;
using United.Challenge.Api.Filters;
using United.Challenge.Api.Middleware;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationProblemDetailsFilter>();
});

// Off automatic 400
builder.Services.Configure<ApiBehaviorOptions>(opts =>
{
    opts.SuppressModelStateInvalidFilter = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache(); // Idempotency


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<IdempotencyMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
