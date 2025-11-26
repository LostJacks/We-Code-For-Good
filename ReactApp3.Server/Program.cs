using System;
using System.Text;
using LabelingTool.BLL.Interfaces;
using LabelingTool.BLL.Services;
using LabelingTool.DAL;
using LabelingTool.DAL.Interfaces;
using LabelingTool.DAL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);


var signingKeyString = builder.Configuration["Jwt:SigningKey"];
if (string.IsNullOrWhiteSpace(signingKeyString))
    throw new InvalidOperationException("Configuration 'Jwt:SigningKey' is missing or empty. Add it to appsettings.json or Secret Manager.");

var issuer = builder.Configuration["Jwt:Issuer"];
if (string.IsNullOrWhiteSpace(issuer))
    throw new InvalidOperationException("Configuration 'Jwt:Issuer' is missing or empty.");

var audience = builder.Configuration["Jwt:Audience"];
if (string.IsNullOrWhiteSpace(audience))
    throw new InvalidOperationException("Configuration 'Jwt:Audience' is missing or empty.");

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKeyString));

builder.Services.AddCors(o => o.AddPolicy("spa", options =>
    options.WithOrigins("http://localhost:5173")
     .AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(o =>
{
    o.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = signingKey,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddSingleton<Database>(sp =>
{
	var config = sp.GetService<IConfiguration>();
    var connectionString = config.GetConnectionString("LabellingDb");
	return new Database(connectionString!);
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordHasher<Model.AppUser>, PasswordHasher<Model.AppUser>>();
builder.Services.AddScoped<IProjectsRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectsService, ProjectsService>();

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("spa");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
