using CommonService.Utility;
using UserService.Services;
using UserService.Utility;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // add the utility servicetoscoped folder 
        ServiceToScope oServiceToScope = new ServiceToScope(builder.Configuration);
        oServiceToScope.AddToScope(builder.Services);

        // Register RabbitMQConnectionHelper
        builder.Services.AddSingleton<RabbitMQConnectionHelper>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}