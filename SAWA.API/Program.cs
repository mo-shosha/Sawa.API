
using Microsoft.EntityFrameworkCore;
using SAWA.API.Middleware;
using SAWA.infrastructure.Data;

namespace SAWA.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCors(op =>
                op.AddPolicy("CORSPolicy", builder =>
                {
                    builder.AllowAnyHeader()
                            .AllowCredentials();
                })
             );
            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<AppDbContext>(options=>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));



            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
