using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Website_Ban_Nuoc_Online.Models
{
    public class WebsiteBanNuocContext : DbContext
    {
        public WebsiteBanNuocContext()
            : base("name=QLY_QUAN_CAFE_Connection")
        {
        }

        public virtual DbSet<KhachHang> KhachHangs { get; set; }
        public virtual DbSet<NhanVien> NhanViens { get; set; }
        public virtual DbSet<Loai> Loais { get; set; }
        public virtual DbSet<Mon> Mons { get; set; }
        public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }
        public virtual DbSet<TinhTrang> TinhTrangs { get; set; }
        public virtual DbSet<HoaDon> HoaDons { get; set; }
        public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Khóa ghép cho ChiTietHoaDon
            modelBuilder.Entity<ChiTietHoaDon>()
                .HasKey(c => new { c.MaHD, c.MaMon });

            // Map tên cột TinhTrang (cột tên TinhTrang, property mình đặt TinhTrangDon)
            modelBuilder.Entity<TinhTrang>()
                .Property(t => t.TinhTrangDon)
                .HasColumnName("TinhTrang");

            base.OnModelCreating(modelBuilder);
        }
    }
}