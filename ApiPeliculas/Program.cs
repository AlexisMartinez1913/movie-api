using ApiPeliculas.Data;
using ApiPeliculas.Repository;
using ApiPeliculas.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ApiPeliculas.MoviesMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using XAct;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ApiPeliculas.Models;
var builder = WebApplication.CreateBuilder(args);

//configurar la conexion a sql server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql"));
});

//soporte para autenticacion con .NET IDENTITY
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

//agregamos los repositorios
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
//seguridad
var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

//se debe de agregar los auto mapers
builder.Services.AddAutoMapper(typeof(MoviesMapper));

// añadir la cache
builder.Services.AddResponseCaching();

//configurar autenticacion
builder.Services.AddAuthentication(
    x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });


// Add services to the container.
//añadir cache global

builder.Services.AddControllers( option =>
{
    option.CacheProfiles.Add("Default20Seconds", new CacheProfile()
    {
        Duration = 30
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//documentacion de autenticacion en swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
        "Jwt authentication using the scheme Bearer.  \r\n\r\\" +
        "Enter the word 'Bearer' followed by a [space] and then your token in the field below \r\n\r\\" +
        "Example: \"Bearer tkdkdkdkdkdkd\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name= "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

//soporte para cors
//se puede habilitar un dominio, multiples dominios
//cualquier dominio - tener en cuenta seguridad
//usamos de ejemplo el dominio http://localhost:3223, se debe cambiar por el correcto
//se usa (*) para todos los dominios
builder.Services.AddCors(p => p.AddPolicy("PolicyCors", build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//soporte para cors
app.UseCors("PolicyCors");
//proteger accesos
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
