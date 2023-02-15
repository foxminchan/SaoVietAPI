using Application.Services;
using AspNetCoreRateLimit;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Prometheus;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.SystemConsole.Themes;
using System.Reflection;
using System.Text;
using Application.Cache;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebAPI.Middlewares;
using HealthCheckService = Application.Health.HealthCheckService;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using Application.Transaction;
using Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

#region Authorization
builder.Services.AddAuthorization();
#endregion

builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
}).AddXmlSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

#region Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "application/octet-stream",
        "application/x-msdownload",
        "application/x-msdos-program",
        "application/x-msmetafile",
        "application/x-ms-shortcut",
    });
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});
#endregion

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

#region Health Check
builder.Services.AddHealthChecks()
    .AddCheck<HealthCheckService>("SaoVietApiChecks", tags: new[] { "Sao Viet Api" });
#endregion

#region Authentication
var tokenValidationParameters = new TokenValidationParameters()
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value,
    ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
        builder.Configuration.GetSection("Jwt:Key").Value ?? throw new InvalidOperationException())),
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = tokenValidationParameters;
    });
builder.Services.AddSingleton(tokenValidationParameters);
#endregion

#region Swagger
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
#endregion

#region Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "SaoVietApi";
});
builder.Services.AddSingleton<ICache, CacheService>();
builder.Services.AddResponseCaching();
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
    options.IpWhitelist = new List<string> { "127.0.0.1", "::1/10", "192.168.5.143/24" };
    options.GeneralRules = new List<RateLimitRule>
    {
        new()
        {
            Endpoint = "POST",
            Period = "10m",
            Limit = 20,
        },
        new()
        {
            Endpoint = "PUT",
            Period = "15m",
            Limit = 15,
        },
        new()
        {
            Endpoint = "DELETE",
            Period = "15m",
            Limit = 10,
        },
        new()
        {
            Endpoint = "GET",
            Period = "15m",
            Limit = 50,
        },
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
    options.UseLoggerFactory(LoggerFactory.Create(log =>
    {
        log.AddConsole();
        ConfigureLogging();
        log.AddSerilog(dispose: true);
    }));
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});
#endregion

#region CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", cors =>
    {
        cors.WithOrigins("https://localhost:5000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
#endregion

#region Mapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
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
builder.Services.AddTransient<AuthorizationService>();
#endregion

#region Transaction
builder.Services.AddTransient<TransactionService>();
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
        swagger.Tags = new List<OpenApiTag> {
            new()
            {
                Name = "Authentication",
                Description = "Xác thực",
                ExternalDocs = findOutMore
            },
            new()
            {
                Name = "Teacher",
                Description = "Quản lý thông tin giáo viên",
                ExternalDocs = findOutMore
            },
            new ()
            {
                Name = "Class",
                Description = "Quản lý thông tin lớp học",
                ExternalDocs = findOutMore
            },
            new ()
            {
                Name = "Branch",
                Description = "Quản lý thông tin chi nhánh",
                ExternalDocs = findOutMore
            },
            new ()
            {
                Name = "Student",
                Description = "Quản lý thông tin học viên",
                ExternalDocs = findOutMore
            },
            new ()
            {
                Name = "Category",
                Description = "Quản lý thông tin danh mục",
                ExternalDocs = findOutMore
            },
            new ()
            {
                Name = "Course",
                Description = "Quản lý thông tin khoá học",
                ExternalDocs = findOutMore
            },
            new ()
            {
                Name = "Lesson",
                Description = "Quản lý thông tin bài học",
                ExternalDocs = findOutMore
            },
            new ()
            {
                Name = "Attendance",
                Description = "Quản lý thông tin điểm danh",
                ExternalDocs = findOutMore
            }
        };
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

#region Security Headers
app.UseSecurityHeadersMiddleware();
#endregion

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRouting();
app.UseHttpMetrics();
app.UseAuthorization();
app.UseIpRateLimiting();
app.MapControllers();
app.MapMetrics();
app.MapHealthChecks("/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    AllowCachingResponses = false,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});
app.UseHealthChecks("/health/live", new HealthCheckOptions()
{
    Predicate = r => r.Tags.Contains("live"),
    AllowCachingResponses = false,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.UseResponseCaching();
app.UseResponseCompression();
app.Run();