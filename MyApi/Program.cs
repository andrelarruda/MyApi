using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyApi.Configurations;
using MyApi.Context;
using MyApi.Interfaces;
using MyApi.Services;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.IdentityModel.Tokens;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container
// Inject services in the .NET pipeline
builder.Services.AddControllers();

// registering the services
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAutoMapper(typeof(MapperConfig));

// Setup the database
builder.Services.AddDbContext<MyApiContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("myAppCors", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>  // Configuration to allow user to send the token through Swagger (avoiding user to use external rest testing clients, such as Insomnia or Postman).
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme { 
        In = ParameterLocation.Header,    // where's the apiKey located
        Name = "Authorization",           // name of the parameter
        Type = SecuritySchemeType.ApiKey  // type of the parameter
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false, // just for testing we are not validating the Audience neither the Issuer, because we didn't informed them on CreateToken (AuthService).
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration.GetSection("AppSettings:Token").Value!
                    ))
    };
}); // verify the signing key

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("myAppCors");

app.UseAuthorization();

app.MapControllers();

app.Run();
