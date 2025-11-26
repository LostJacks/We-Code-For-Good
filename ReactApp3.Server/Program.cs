using LabelingTool.BLL.BBServices;
using LabelingTool.BLL.ExportServices;
using LabelingTool.DAL.BBRepositories;
using LabelingTool.DAL.BBRepositories.MockData;
using LabelingTool.DAL.GeneralRepositories;
using LabelingTool.DAL.GeneralRepositories.MockData;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IBoundingBoxRepository, MockBoundingBoxRepository>();
builder.Services.AddScoped<IMediaRepository, MockMediaRepository>();
builder.Services.AddScoped<ILabelClassRepository, MockLabelClassRepository>();
builder.Services.AddScoped<IProjectRepository, MockProjectRepository>();

builder.Services.AddScoped<IBoundingBoxService, BoundingBoxService>();
builder.Services.AddScoped<IYoloExportService, YoloExportService>();

builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

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
