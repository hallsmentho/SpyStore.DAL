using Microsoft.EntityFrameworkCore;
using SpyStore.Models.Entities;

namespace SpyStore.DAL.EF
{
    public class StoreContext : DbContext
    {
        public StoreContext()
        {

        }

        public StoreContext(DbContextOptions options) : base(options)
        {


        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingCartRecord> ShoppingCartRecords {get;set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    @"SERVER=DESKTOP-0F3RB1A\SQLEXPRESS2017;DATABASE=SpyStore;TRUSTED_CONNECTION=True;MultipleActiveResultSets=true;"
                    , options => options.ExecutionStrategy(c => new MyExecutionStrategy(c))
                    );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.EmailAddress).HasName("IX_Customers").IsUnique();
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

                entity.Property(e => e.ShipDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.LineItemTotal)
                .HasColumnType("money")
                .HasComputedColumnSql("[Quantity]*[UnitCost]");
                entity.Property(e => e.UnitCost).HasColumnType("money");
                
            });

            modelBuilder.Entity<ShoppingCartRecord>(entity =>
            {
                entity.HasIndex(e => new { ShoppingCartRecordId = e.Id, e.ProductId, e.CustomerId })
                .HasName("IX_ShoppingCart")
                .IsUnique();

                entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Quantity).HasDefaultValue(1);

            });

        }

    }
}
