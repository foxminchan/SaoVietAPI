using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Application.Services;
using AspNetCoreRateLimit;
using Prometheus;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.SystemConsole.Themes;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region Swagger
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    options.AddSecurityDefinition(name: "OAuth2", securityScheme: new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://localhost:5000/connect/authorize"),
                TokenUrl = new Uri("https://localhost:5000/connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "WebAPI", "WebAPI" }
                }
            }
        }
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});
#endregion

#region Request Throttling
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.HttpStatusCode = 429;
    options.RealIpHeader = "X-Real-IP";
    options.ClientIdHeader = "X-ClientId";
    options.GeneralRules = new List<RateLimitRule>
    {
        new()
        {
            Endpoint = "*",
            Period = "1m",
            Limit = 20,
        }
    };
});
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddInMemoryRateLimiting();
#endregion

#region DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseLoggerFactory(LoggerFactory.Create(log => log.AddConsole()));
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});
#endregion

#region CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", cors =>
    {
        cors.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
#endregion

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

#region ELK Stack
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
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
            theme: AnsiConsoleTheme.Literate)
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
#endregion

#region Services
builder.Services.AddTransient<TeacherService>();
builder.Services.AddTransient<ClassService>();
builder.Services.AddTransient<BranchService>();
builder.Services.AddTransient<StudentService>();
builder.Services.AddTransient<CategoryService>();
builder.Services.AddTransient<CourseService>();
builder.Services.AddTransient<LessonService>();
builder.Services.AddTransient<AttendanceService>();
#endregion

var app = builder.Build();

#region Swagger UI
var info = new OpenApiInfo
{
    Title = "Sao Việt API",
    Version = "1.0.0",
    Description = "API cho ứng dụng quản lý trung tâm tin học Sao Việt. Mọi thắc mắc xin liên hệ theo địa chỉ email nguyenxuannhan407@gmail.com hoặc nd.anh@hutech.edu.vn",
    Contact = new OpenApiContact
    {
        Name = "Nguyễn Xuân Nhân",
        Email = "nguyenxuannhan407@gmail.com",
        Url = new Uri("https://www.facebook.com/FoxMinChan/")
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

var findOutMore = new OpenApiExternalDocs
{
    Description = "Tìm hiểu thêm về Swagger",
    Url = new Uri("https://swagger.io/"),
};

var teacherTag = new OpenApiTag
{
    Name = "Teacher",
    Description = "Quản lý thông tin giáo viên",
    ExternalDocs = findOutMore
};

var classTag = new OpenApiTag
{
    Name = "Class",
    Description = "Quản lý thông tin lớp học",
    ExternalDocs = findOutMore
};

var branchTag = new OpenApiTag
{
    Name = "Branch",
    Description = "Quản lý thông tin chi nhánh",
    ExternalDocs = findOutMore
};

var studentTag = new OpenApiTag
{
    Name = "Student",
    Description = "Quản lý thông tin học viên",
    ExternalDocs = findOutMore
};
var categoryTag = new OpenApiTag
{
    Name = "Category",
    Description = "Quản lý thông tin danh mục",
    ExternalDocs = findOutMore
};
var lessonTag = new OpenApiTag
{
    Name = "Lesson",
    Description = "Quản lý thông tin bài học",
    ExternalDocs = findOutMore
};
var courseTag = new OpenApiTag
{
    Name = "Course",
    Description = "Quản lý thông tin khoá học",
    ExternalDocs = findOutMore
};

app.UseStaticFiles();
app.UseCors("AllowAll");

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
        swagger.Tags = new List<OpenApiTag> { teacherTag, classTag, branchTag, studentTag, categoryTag, courseTag, lessonTag };
        swagger.Servers = new List<OpenApiServer>
        {
            new()
            {
                Url = $"{httpReq.Scheme}://{httpReq.Host.Value}"
            }
        };
    });
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sao Việt API v1");
    c.InjectStylesheet("/css/swagger-ui.css");
    c.InjectJavascript("/js/swagger-ui.js");
    c.DocumentTitle = "Sao Việt API";
    c.OAuthConfigObject = new OAuthConfigObject
    {
        ClientId = "swagger",
        AppName = "Swagger",
        UsePkceWithAuthorizationCodeGrant = true,
        UseBasicAuthenticationWithAccessCodeGrant = true
    };
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
#endregion

app.UseAuthentication();
app.UseRouting();
app.UseHttpMetrics();
app.UseAuthorization();
app.UseIpRateLimiting();
app.MapControllers();
app.MapMetrics();
app.Run();