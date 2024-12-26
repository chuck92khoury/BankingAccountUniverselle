using Microsoft.EntityFrameworkCore;


namespace BankingPlatformAPI.Data
{
    public class BankingDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Héritage pour Client
            modelBuilder.Entity<Client>()
                .HasDiscriminator<string>("ClientType")
                .HasValue<IndividualClient>("Individual")
                .HasValue<ProfessionalClient>("Professional");

            // Relation entre BankAccount et Transaction
            modelBuilder.Entity<BankAccount>()
                .HasMany(b => b.Transactions)
                .WithOne(t => t.BankAccount)
                .HasForeignKey(t => t.BankAccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
