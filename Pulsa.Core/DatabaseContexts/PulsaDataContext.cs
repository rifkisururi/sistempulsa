 using Microsoft.EntityFrameworkCore;
using Pulsa.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsa.Data
{
    public class PulsaDataContext : DbContext
    {
        public PulsaDataContext(DbContextOptions<PulsaDataContext> options):base(options) { }
        //protected override void OnModelCreating(ModelBuilder modelBuilder) {
        //    modelBuilder.UseSerialColums();
        //}
        public DbSet<Pengguna> penggunas { get; set; }
        public DbSet<Tagihan_master> tagihan_masters { get; set; }
        public DbSet<Tagihan_detail> tagihan_details { get; set; }
    }
}
