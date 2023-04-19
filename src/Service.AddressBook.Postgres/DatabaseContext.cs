using Microsoft.EntityFrameworkCore;
using MyJetWallet.Sdk.Postgres;
using Service.AddressBook.Domain.Models;

namespace Service.AddressBook.Postgres
{
    public class DatabaseContext : MyDbContext
    {
        public const string Schema = "addressbook";

        public const string AddressBookRecordsTableName = "addressbok";

        public DbSet<AddressBookRecord> AddressBook { get; set; }
        
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder.Entity<AddressBookRecord>().ToTable(AddressBookRecordsTableName);
            modelBuilder.Entity<AddressBookRecord>().HasKey(e => new {e.OwnerClientId, ClientId = e.ContactClientId});
            modelBuilder.Entity<AddressBookRecord>().Property(e => e.LastTs).HasDefaultValue(DateTime.MinValue);
            modelBuilder.Entity<AddressBookRecord>().Property(e => e.Nickname).IsRequired();
            modelBuilder.Entity<AddressBookRecord>().Property(e => e.Name).IsRequired();
            modelBuilder.Entity<AddressBookRecord>().Property(e => e.Iban).IsRequired(false);
            
            modelBuilder.Entity<AddressBookRecord>().HasIndex(e => new {e.OwnerClientId, e.Nickname}).IsUnique();
            modelBuilder.Entity<AddressBookRecord>().HasIndex(e => new {e.OwnerClientId, e.Name}).IsUnique();
            modelBuilder.Entity<AddressBookRecord>().HasIndex(e => new {e.OwnerClientId, e.Iban}).IsUnique();

            base.OnModelCreating(modelBuilder);
        }

        public async Task<int> UpsertAsync(IEnumerable<AddressBookRecord> entities)
        {
            var result = await AddressBook.UpsertRange(entities).AllowIdentityMatch().RunAsync();
            return result;
        }
        
    }
}
