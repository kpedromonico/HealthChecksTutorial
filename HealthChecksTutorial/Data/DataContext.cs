using HealthChecksTutorial.Data.DTO;
using Microsoft.EntityFrameworkCore;

namespace HealthChecksTutorial.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
            
        }

        public DbSet<SuperheroDTO> Superheroes { get; set; }
    }
}