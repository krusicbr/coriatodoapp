
using CoriaToDo.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace CoriaToDo.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Load environment variables from .env file (if it exists)
            DotEnv.Load();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ToDoDbContext>(options =>
               options.UseNpgsql(ChangeDbHostNameIfNeeded(builder.Configuration.GetConnectionString("PostgresDefaultConnection"))));

            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                MigrateDb(app);
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        public static string ChangeDbHostNameIfNeeded(string connString)
        {
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST_NAME");
            if (!string.IsNullOrEmpty(dbHost))
            {
                connString = connString.Replace("localhost", dbHost);
            }
            return connString;
        }

        static void MigrateDb(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            scope.ServiceProvider.GetService<ToDoDbContext>().Database.Migrate();
        }
    }
}