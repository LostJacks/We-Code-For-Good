using System;
using System.Text;
using LabelingTool.BLL.ExportServices;
using LabelingTool.DAL.GeneralRepositories;
using LabelingTool.BLL.Interfaces;
using LabelingTool.BLL.Services;
using LabelingTool.BLL.BBServices;
using LabelingTool.DAL;
using LabelingTool.DAL.Interfaces;
using LabelingTool.DAL.Repositories;
using LabelingTool.DAL.BBRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// =============================================================
// ? JWT CONFIGURATION
// =============================================================
var signingKeyString = builder.Configuration["Jwt:SigningKey"];
if (string.IsNullOrWhiteSpace(signingKeyString))
    throw new InvalidOperationException("Configuration 'Jwt:SigningKey' is missing or empty.");

var issuer = builder.Configuration["Jwt:Issuer"];
if (string.IsNullOrWhiteSpace(issuer))
    throw new InvalidOperationException("Configuration 'Jwt:Issuer' is missing or empty.");

var audience = builder.Configuration["Jwt:Audience"];
if (string.IsNullOrWhiteSpace(audience))
    throw new InvalidOperationException("Configuration 'Jwt:Audience' is missing or empty.");

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKeyString));

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

// =============================================================
// ? CORS CONFIGURATION
// =============================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// =============================================================
// ? DATABASE + CORE SERVICES
// =============================================================
builder.Services.AddSingleton<Database>(sp =>
{
    var config = sp.GetService<IConfiguration>();
    var connectionString = config.GetConnectionString("LabellingDb");
    return new Database(connectionString!);
});

// =============================================================
// ? NORMAL SERVICES (Auth, Users, Projects)
// =============================================================
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProjectsRepository, LabelingTool.DAL.Repositories.ProjectRepository>();
builder.Services.AddScoped<IProjectsService, ProjectsService>();
builder.Services.AddScoped<IPasswordHasher<Model.AppUser>, PasswordHasher<Model.AppUser>>();
builder.Services.AddScoped<IMediaRepository, MediaRepository>();
builder.Services.AddScoped<ILabelClassRepository, LabelClassRepository>();
builder.Services.AddScoped<IProjectRepository, LabelingTool.DAL.GeneralRepositories.ProjectRepository>();

// =============================================================
// ? BOUNDING BOX SERVICES
// =============================================================
builder.Services.AddScoped<IBoundingBoxRepository, BoundingBoxRepository>();
builder.Services.AddScoped<IBoundingBoxService, BoundingBoxService>();
builder.Services.AddScoped<IYoloExportService, YoloExportService>();

// =============================================================
// ? CONTROLLERS + SWAGGER
// =============================================================
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

// =============================================================
// ? APP PIPELINE
// =============================================================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReact");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
