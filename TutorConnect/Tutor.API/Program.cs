using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Tutor.Applications;
using Tutor.Applications.HUBS;
using Tutor.Applications.Interfaces;
using Tutor.Applications.Services;
using Tutor.Infratructures;
using Tutor.Infratructures.Persistence;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configuration
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        // Add services to the container.
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        });
        // Database
        builder.Services.AddDatabaseServices(builder.Configuration);
        //OpenAI
        //Add dependencies
        builder.Services.AddInfratructure().AddApplication();
        // SignalR
        builder.Services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
        });
        //config enum
        builder.Services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });


        // JWT Authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                ClockSkew = TimeSpan.Zero
            };
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine("Token validated successfully");
                    return Task.CompletedTask;
                },
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chatHub") || path.StartsWithSegments("/videocallhub")))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        })
        .AddGoogle(googleOptions =>
        {
            googleOptions.SignInScheme = "Cookies";
            googleOptions.ClientId = builder.Configuration["Google:ClientId"];
            googleOptions.ClientSecret = builder.Configuration["Google:ClientSecret"];
        });
        ;

        builder.Services.AddAuthentication()
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/api/GoogleAuth/login";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

        // SignalR
        builder.Services.AddSignalR(hubOptions =>
        {
            hubOptions.EnableDetailedErrors = true;
        });

        // Logging
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.AddHttpClient<IOpenAIService, OpenAIService>();
        // Controllers
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        // CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder => builder.WithOrigins(
                    "http://localhost:5173", "http://localhost:5174",
                    "https://spring2025-swd-392-net-1704-3-fe.vercel.app", "https://tutoring.id.vn")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
        });


        // Swagger Configuration
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Ngô Văn Hưng API",
                Version = "v2",
                Description = "Ngô Văn Hưng có đẹp trai không"
            });

            // JWT Authentication configuration in Swagger
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            };

            options.AddSecurityDefinition("Bearer", securityScheme);
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
                    Array.Empty<string>()
                }
            });
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

        });
        TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var app = builder.Build();
        app.Use(async (context, next) =>
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("vi-VN");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("vi-VN");
            await next();
        });

        // Configure Swagger UI
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Electronic API V1");
        });

        // Development-specific settings
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // HTTPS Redirection
        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseCors("AllowSpecificOrigin");
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<ChatHub>("/chatHub");
            endpoints.MapHub<ChatGPTHub>("/chatGPTHub");
            endpoints.MapHub<VideoCallHub>("/videocallhub");
            endpoints.MapControllers();
        });

        // Run the app
        app.Run();
    }
}
