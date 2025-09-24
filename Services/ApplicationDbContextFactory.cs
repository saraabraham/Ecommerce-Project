using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DotNetEnv;
using ElectronicsStoreMVC.Models;
using ElectronicsStoreMVC.Services;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Load .env variables
        Env.Load();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Use design-time connection string
        string connectionString = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION")
                                  ?? "Server=127.0.0.1;Port=3306;Database=ElectronicsStore;User=root;Password=021297bd;SslMode=Preferred;";

        optionsBuilder.UseMySQL(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
