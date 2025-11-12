using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization; // ✅ ADD THIS
using LabelingTool.DAL;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add controllers (API endpoints) + fix circular reference issue
builder.Services.AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // ✅ Fix for Project ↔ Labels loop
        x.JsonSerializerOptions.WriteIndented = true;
    });

// ✅ Add Swagger for API testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Enable CORS (allow frontend requests)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ✅ Register your EF Core database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

/* ✅ Enable detailed error pages (development mode) */
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Serve static files (for /wwwroot/uploads etc.)
app.UseStaticFiles();

// ✅ Enable routing & CORS
app.UseRouting();
app.UseCors();

// ✅ Map API controllers
app.MapControllers();

// ✅ Run the web app
app.Run();
