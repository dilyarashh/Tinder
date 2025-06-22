using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Tinder.DBContext;
using Tinder.Services;
using Tinder.Services.IServices;
using Tinder.SupportiveServices.Exceptions;
using Tinder.SupportiveServices.Password;
using Tinder.SupportiveServices.Token;
using Tinder.SupportiveServices.Token.RemoveTokens;
using QuartzHostedService = Quartz.QuartzHostedService;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Configure(builder.Configuration.GetSection("Kestrel"));
});

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbcontext>(options => options.UseNpgsql(connection));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
builder.Services.AddLogging(builder => builder.AddConsole().AddDebug());
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddSingleton<IJobFactory, JobFactory>(); 
builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
builder.Services.AddScoped<ExpiredTokenRemovalJob>(); 
builder.Services.AddSingleton(new JobSchedule(
    jobType: typeof(ExpiredTokenRemovalJob),
    cronExpression: "0 0 * * *")); 
builder.Services.AddHostedService<QuartzHostedService>();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

builder.Services.AddSwaggerGen(options =>
{ 
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Please enter token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
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
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
    options.EnableAnnotations();
});

builder.Services.AddAuthorization(services =>
{
    services.AddPolicy("TokenBlackListPolicy", policy => policy.Requirements.Add(new TokenBlackListRequirment()));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin() 
                .AllowAnyMethod() 
                .AllowAnyHeader(); 
        });
});

builder.Services.AddSingleton<TokenInteractions>();
builder.Services.AddSingleton<IAuthorizationHandler, TokenBlackListPolicy>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserPreferenceService, UserPreferenceService>();
builder.Services.AddScoped<HashPassword>();

var app = builder.Build();

try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbcontext>();
    await dbContext.Database.MigrateAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка при миграции базы данных: {ex.Message}");
}

app.UseHttpsRedirection(); 
app.UseCors("AllowAllOrigins"); 
app.UseAuthentication();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization(); 
app.MapControllers();
app.UseMiddleware<Middleware>();
app.Run();