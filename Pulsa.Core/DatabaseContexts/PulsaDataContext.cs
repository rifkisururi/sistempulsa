using Microsoft.EntityFrameworkCore;
using Pulsa.Domain.Entities;

namespace Pulsa.Data
{
    public class PulsaDataContext : DbContext
    {
        public PulsaDataContext(DbContextOptions<PulsaDataContext> options) : base(options) {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }
        //protected override void OnModelCreating(ModelBuilder modelBuilder) {
        //    modelBuilder.UseSerialColums();
        //}
        public DbSet<Pengguna> penggunas { get; set; }
        public DbSet<Tagihan_master> tagihan_masters { get; set; }
        public DbSet<Tagihan_detail> tagihan_details { get; set; }
        public DbSet<TopUp> topups { get; set; }
        public DbSet<TopUp_metode> topup_Metodes { get; set; }
        public DbSet<user_saldo_history_detail> user_saldo_history_detail { get; set; }
        public DbSet<Provider_h2h> Provider_h2h { get; set; }
        public DbSet<Supplier_produk> supplier_produks { get; set; }
        public DbSet<Produk> produks { get; set; }

    }
}
