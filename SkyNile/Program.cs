using BusinessLogic.Models;
using DataAccess.Data;
using Hangfire;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SkyNile.HelperModel;
using SkyNile.Infrastructure;
using SkyNile.Services;
using SkyNile.Services.Interfaces;
using SkyNile.Settings;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddTransient<IMailingServices, MailingServices>();
builder.Services.AddTransient<IFlightSchedulingService, FlightSchedulingService>();
builder.Services.AddTransient<ISearchService, FlightSearchService>();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseLazyLoadingProxies().UseSqlServer(connectionString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SkyNile",
        Version = "v1",
        Description = "Backend API for An airline company is a business that provides air transport services for passengers.",
    });
    options.EnableAnnotations();
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (JWT). Example: bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"

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
         new string[] { }
     }
 });
    options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
});
builder.Services.AddControllers();
builder.Services.AddControllers()
   .AddJsonOptions(options =>
   {
       options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
   });

builder.Services.AddMapster();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.SaveToken = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
        };
    });
builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();
<<<<<<< HEAD
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
=======
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
    options.TokenLifespan = TimeSpan.FromMinutes(5));

>>>>>>> 4c144bcb74fb16ce54f4bd1abc7b62fa1714400d
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment()) { }
//app.UseMigrationsEndPoint();
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard("/dashboard");
app.UseExceptionHandler();
app.MapControllers();

app.Run();


public static class MapsterConfiguration
{
    public static IServiceCollection AddMapster(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly()); // Scan for all mapping configurations

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}
