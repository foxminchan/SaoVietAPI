using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Application.Services;
using Prometheus;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    {
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", cors =>
    {
        cors.AllowAnyOrigin()
        .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// ELK Stack Configuration
ConfigureLogging();
builder.Host.UseSerilog();

void ConfigureLogging()
{
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile(
            $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
            optional: true)
        .Build();

    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .WriteTo.Debug()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment!))
        .Enrich.WithProperty("Environment", environment!)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

ElasticsearchSinkOptions ConfigureElasticSink(IConfiguration configuration, string? environment)
{
    return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"] ?? string.Empty))
    {
        AutoRegisterTemplate = true,
        IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name?.ToLower(System.Globalization.CultureInfo.CurrentCulture).Replace(".", "-")}-{environment?.ToLower(System.Globalization.CultureInfo.CurrentCulture).Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
    };
}

builder.Services.AddTransient<TeacherService>();
builder.Services.AddTransient<ClassService>();

var app = builder.Build();

var info = new OpenApiInfo
{
    Title = "Sao Việt API",
    Version = "v1",
    Description = "API cho ứng dụng quản lý trung tâm tin học Sao Việt. Mọi thắc mắc xin liên hệ theo địa chỉ email nguyenxuannhan407@gmail.com hoặc nd.anh@hutech.edu.vn",
    Contact = new OpenApiContact
    {
        Name = "Nguyễn Xuân Nhân",
        Email = "nguyenxuannhan407@gmail.com",
    },
    License = new OpenApiLicense
    {
        Name = "MIT",
        Url = new Uri("https://opensource.org/licenses/MIT"),
    },
    TermsOfService = new Uri("https://sites.google.com/view/trungtamtinhocsaoviet"),
};

var externalDocs = new OpenApiExternalDocs
{
    Description = "Về trung tâm tin học Sao Việt",
    Url = new Uri("https://blogdaytinhoc.com/"),
};

var teacherTag = new OpenApiTag
{
    Name = "Teacher",
    Description = "Quản lý thông tin giáo viên"
};

var classTag = new OpenApiTag
{
    Name = "Class",
    Description = "Quản lý thông tin lớp học"
};

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
        c.SerializeAsV2 = true;
        c.PreSerializeFilters.Add((swagger, httpReq) =>
        {
            if (httpReq is null)
                throw new ArgumentNullException(nameof(httpReq));
            swagger.Info = info;
            swagger.ExternalDocs = externalDocs;
            swagger.Tags = new List<OpenApiTag> { teacherTag, classTag };
        });

    });

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sao Việt API v1");
    });
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseHttpMetrics();

app.UseAuthorization();

app.MapControllers();

app.MapMetrics();

app.Run();