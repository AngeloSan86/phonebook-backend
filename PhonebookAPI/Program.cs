using Microsoft.EntityFrameworkCore;
using PhonebookAPI.Data;
using PhonebookAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ========== CONFIGURA ENTITY FRAMEWORK E MYSQL ==========
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ========== CONFIGURA JWT AUTHENTICATION ==========
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
              Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
      };
    });

// ========== REGISTRA I SERVICES ==========
builder.Services.AddScoped<ITokenService, TokenService>();

// ========== CONFIGURA CORS (per Angular in locale E in produzione) ==========
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend", policy =>
  {
    policy.WithOrigins(
            "http://localhost:4200",           // Sviluppo locale
            "https://*.vercel.app",            // Vercel (tutti i sottodomini)
            "https://rubrica-phonebook.vercel.app" // Il tuo URL specifico (aggiornalo dopo il deploy)
        )
        .SetIsOriginAllowedToAllowWildcardSubdomains() // Permette *.vercel.app
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
  });
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
  // Configurazione per JWT in Swagger
  options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    Name = "Authorization",
    Type = SecuritySchemeType.Http,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "Inserisci il token JWT nel formato: Bearer {token}"
  });

  options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ========== USA CORS (una sola volta!) ==========
app.UseCors("AllowFrontend");

// ========== USA AUTHENTICATION E AUTHORIZATION ==========
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
