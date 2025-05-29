using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Data
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        // Constructor used by ASP.NET Core DI container
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<SuperLottoUserPick> SuperLottoUserPicks { get; set; }
        public virtual DbSet<SuperLottoWinningNumberDTO> WinningPicksFromProc { get; set; }
        public virtual DbSet<SuperLottoWinningNumber> SuperLottoWinningNumbers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SuperLottoWinningNumberDTO>()
                .HasNoKey()
                .ToView(null);

            modelBuilder.Entity<SuperLottoUserPick>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
                entity.Property(e => e.RowCheckSum).HasComputedColumnSql("([LottoChecker].[SuperLottoUserPicksRowCheckSum]([UserId],[FirstPick],[SecondPick],[ThirdPick],[FourthPick],[FifthPick],[MegaPick]))", false);
                entity.Property(e => e.DateAdded).HasDefaultValueSql("GETDATE()").ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SuperLottoWinningNumber>(entity =>
            {
                entity.Property(e => e.DrawNumber).ValueGeneratedNever();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
