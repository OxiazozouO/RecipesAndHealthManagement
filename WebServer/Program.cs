using Microsoft.EntityFrameworkCore;
using WebServer.Configurations;
using WebServer.DatabaseModel;
using WebServer.Helper;

public class Program
{
    public static void Main(string[] args)
    {
        AppSettings.LoadByFile();

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //数据库上下文
        builder.Services
            .AddDbContext<RecipeAndHealthSystemContext>(m =>
                m.UseMySQL(AppSettings.ConnectionString));

        JwtHelper.AddJwtService(AppSettings.JwtConfig, builder.Services);

        // 允许所有来源（仅限开发环境）
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
        }

        app.UseCors("AllowAll");

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseStaticFiles();

        app.MapControllers();

        app.Run();
    }
}