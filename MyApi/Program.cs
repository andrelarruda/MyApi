using Microsoft.EntityFrameworkCore;
using MyApi.Configurations;
using MyApi.Context;
using MyApi.Interfaces;
using MyApi.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container
// Inject services in the .NET pipeline
builder.Services.AddControllers();

// registering the services
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAutoMapper(typeof(MapperConfig));

// Setup the database
builder.Services.AddDbContext<MyApiContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
