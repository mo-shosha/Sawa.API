using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using SAWA.API.Middleware;
using SAWA.core.Interfaces;
using SAWA.core.IServices;
using SAWA.core.Models;
using SAWA.infrastructure.Data;
using SAWA.infrastructure.Repositories;
using SAWA.infrastructure.Services;
using Serilog;


namespace SAWA.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("wwwroot/Logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Add CORS policy
            builder.Services.AddCors(options =>
                options.AddPolicy("CORSPolicy", policy =>
                {
                     policy.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                })
            );

            // Add Identity services (for authentication and authorization)
            builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // Add scoped and singleton services
            builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));
            builder.Services.AddScoped<IGenerateTokenServices, GenerateTokenServices>();
            builder.Services.AddScoped<IFileManagementService, FileManagementService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IEmailServices, EmailServices>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


            // Add Memory Cache
            builder.Services.AddMemoryCache(); 

            // Add controllers
            builder.Services.AddControllers();

            // Configure database context with SQL Server
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
            );

            // Add Swagger and API Explorer for documentation
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure Authentication (use a helper method for this)
            builder.Services.ConfigureAuthentication(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline
            //if (app.Environment.IsDevelopment())
            
            app.UseSwagger();
            app.UseSwaggerUI();
            

            // Use custom middleware for exception handling
            app.UseMiddleware<ExceptionMiddleware>();

            // Additional middlewares
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            // Map controllers to routes
            app.MapControllers();

            // Run the application
            app.Run();
        }
    }
}
