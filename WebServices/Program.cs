using Aplicacion.Core;
using Aplicacion.Services;
using Aplicacion.Services.whatsapp;
using Infraestructura.Context;
using Infraestructura.Core.Jwtoken;
using Infraestructura.Core.RestClient;
using Microsoft.EntityFrameworkCore;
using WebServices.Controllers;
using WebServices.Jwtoken;
using WebServices.Middleware;
using WebServices.RegisterContainer;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.ConfigureJwt();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

const string AllowAllOriginsPolicy = "AllowAllOriginsPolicy";

var corsPolicy = "_myCorsPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicy,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Agrega tu frontend
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Importante para SignalR
        });
});


builder.Services.RegisterApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseCors(corsPolicy);
app.MapHub<ChatHub>("/chatHub");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
