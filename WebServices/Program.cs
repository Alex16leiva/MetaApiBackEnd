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


string conectionString = builder.Configuration.GetConnectionString("conectionDataBase");

builder.Services.AddDbContext<AppDbContext>(
        dbContextOption => dbContextOption.UseSqlServer(conectionString), ServiceLifetime.Transient
    );

builder.Services.AddTransient<IDataContext, AppDbContext>();
builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

//Register Json Web Token
builder.Services.AddTransient<ITokenService, JwtTokenService>();


RestClientFactory.SetCurrent(new HttpRestClientFactory());
//builder.Services.AddTransient<IRestClient, HttpRestClient>();
//builder.Services.AddTransient<IRestClientFactory, HttpRestClientFactory>();

builder.Services.AddScoped<SecurityAplicationService>();
builder.Services.AddScoped<WhatsappAppService>();

builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors(corsPolicy);
app.MapHub<ChatHub>("/chatHub");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
