﻿using Microsoft.EntityFrameworkCore;
using Pulsa.Domain.Entities;

namespace Pulsa.Data
{
    public class PulsaDataContext : DbContext
    {
        public PulsaDataContext(DbContextOptions<PulsaDataContext> options) : base(options) { }
        //protected override void OnModelCreating(ModelBuilder modelBuilder) {
        //    modelBuilder.UseSerialColums();
        //}
        public DbSet<Pengguna> penggunas { get; set; }
        public DbSet<Tagihan_master> tagihan_masters { get; set; }
        public DbSet<Tagihan_detail> tagihan_details { get; set; }
        public DbSet<TopUp> topups { get; set; }
        public DbSet<TopUp_metode> topup_Metodes { get; set; }
    }
}
