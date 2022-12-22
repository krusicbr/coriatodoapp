
using CoriaToDo.API.Data;
using Microsoft.EntityFrameworkCore;
namespace CoriaToDo.API
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ToDoDbContext>(options =>
               options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresDefaultConnection")));

            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsEnvironment("Local") || app.Environment.IsDevelopment())
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

        static void MigrateDb(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            scope.ServiceProvider.GetService<ToDoDbContext>().Database.Migrate();
        }
    }
}