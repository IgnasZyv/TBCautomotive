// using CarHostingWeb.Models;
// using Microsoft.EntityFrameworkCore;
//
// namespace CarHostingWeb.Data
// {
//     public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
//     {
//         // Define your DbSet properties here, for example:
//         // public DbSet<YourModel> YourModels { get; set; }
//
//         public DbSet<Car> Cars { get; set; }
//         
//         protected override void OnModelCreating(ModelBuilder modelBuilder)
//         {
//             base.OnModelCreating(modelBuilder);
//     
//             // Only configure the Id to use PostgreSQL's identity column
//             modelBuilder.Entity<Car>()
//                 .Property(c => c.Id)
//                 .UseIdentityAlwaysColumn();
//         
//             // Remove any created_at configuration here to avoid conflicts
//         }
//     }    
// }