using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMvcProject.Data.Database
{
    public class DatabaseFactory:IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            //optionsBuilder.UseSqlite("Filename=Data/TestDatabase.db");
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new DatabaseContext(optionsBuilder.Options);

        }
    }
}
