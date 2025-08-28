using API_Coding_Exercise.DataLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Coding_Exercise.DataLibrary.Data
{
    public class PizzaDbContext : DbContext
    {
        public PizzaDbContext(DbContextOptions<PizzaDbContext> options) : base(options) { }

        public DbSet<PizzaModel> Pizzas { get; set; }
        public DbSet<ToppingModel> Toppings { get; set; }
        public DbSet<PizzaTopping> PizzaToppings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PizzaTopping>()
                .HasKey(pt => new { pt.PizzaId, pt.ToppingId });

            modelBuilder.Entity<PizzaTopping>()
                .HasOne(pt => pt.Pizza)
                .WithMany(p => p.PizzaToppings)
                .HasForeignKey(pt => pt.PizzaId);

            modelBuilder.Entity<PizzaTopping>()
                .HasOne(pt => pt.Topping)
                .WithMany(t => t.PizzaToppings)
                .HasForeignKey(pt => pt.ToppingId);

            
            modelBuilder.Entity<PizzaModel>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).ValueGeneratedOnAdd();
                entity.HasIndex(p => p.Name).IsUnique();
                entity.Property(p => p.BasePrice).HasPrecision(6, 2);
            });

            
            modelBuilder.Entity<ToppingModel>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Id).ValueGeneratedOnAdd();
                entity.HasIndex(t => t.Name).IsUnique();
                entity.Property(t => t.Price).HasPrecision(6, 2);
            });

            
            modelBuilder.Entity<ToppingModel>().HasData(
                new ToppingModel { Id = 1, Name = "Ham", Description = "Sweet Filipino-style ham", Price = 35.00m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new ToppingModel { Id = 2, Name = "Pineapple", Description = "Fresh pineapple chunks - Filipino favorite!", Price = 25.00m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new ToppingModel { Id = 3, Name = "Longganisa", Description = "Filipino sweet sausage", Price = 45.00m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new ToppingModel { Id = 4, Name = "Bacon", Description = "Crispy bacon strips", Price = 40.00m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new ToppingModel { Id = 5, Name = "Kesong Puti", Description = "Filipino white cheese", Price = 30.00m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new ToppingModel { Id = 6, Name = "Spinach", Description = "Fresh spinach leaves", Price = 20.00m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new ToppingModel { Id = 7, Name = "Chorizo de Bilbao", Description = "Spanish-style sausage", Price = 50.00m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new ToppingModel { Id = 8, Name = "Mushrooms", Description = "Fresh button mushrooms", Price = 25.00m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );

            
            modelBuilder.Entity<PizzaModel>().HasData(
                new PizzaModel { Id = 1, Name = "All Meat Special", Description = "Loaded with ham, bacon, longganisa, and chorizo - meat lover's dream", BasePrice = 299.00m, Size = "Medium", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new PizzaModel { Id = 2, Name = "Hawaiian Delight", Description = "Classic ham and pineapple combination - Filipino style", BasePrice = 249.00m, Size = "Medium", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new PizzaModel { Id = 3, Name = "Spinach Garden", Description = "Fresh spinach with kesong puti and mushrooms", BasePrice = 229.00m, Size = "Medium", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );

            
            modelBuilder.Entity<PizzaTopping>().HasData(
                
                new PizzaTopping { PizzaId = 1, ToppingId = 1 },
                new PizzaTopping { PizzaId = 1, ToppingId = 3 },  
                new PizzaTopping { PizzaId = 1, ToppingId = 4 },
                new PizzaTopping { PizzaId = 1, ToppingId = 7 },

                new PizzaTopping { PizzaId = 2, ToppingId = 1 },
                new PizzaTopping { PizzaId = 2, ToppingId = 2 },

                new PizzaTopping { PizzaId = 3, ToppingId = 6 },
                new PizzaTopping { PizzaId = 3, ToppingId = 5 },
                new PizzaTopping { PizzaId = 3, ToppingId = 8 }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}