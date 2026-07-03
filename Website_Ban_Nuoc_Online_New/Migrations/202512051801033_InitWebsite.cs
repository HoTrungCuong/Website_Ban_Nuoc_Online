namespace Website_Ban_Nuoc_Online_New.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitWebsite : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Admins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 50),
                        FullName = c.String(nullable: false, maxLength: 120),
                        Password = c.String(nullable: false, maxLength: 255),
                        Email = c.String(nullable: false, maxLength: 150),
                        Phone = c.String(maxLength: 20),
                        CCCD = c.String(maxLength: 20),
                        Gender = c.String(maxLength: 10),
                        Address = c.String(maxLength: 255),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChiTietHoaDons",
                c => new
                    {
                        MaHD = c.Int(nullable: false),
                        MaMon = c.Int(nullable: false),
                        SoLuong = c.Int(nullable: false),
                        GiaBan = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.MaHD, t.MaMon })
                .ForeignKey("dbo.HoaDons", t => t.MaHD, cascadeDelete: true)
                .ForeignKey("dbo.Mons", t => t.MaMon, cascadeDelete: true)
                .Index(t => t.MaHD)
                .Index(t => t.MaMon);
            
            CreateTable(
                "dbo.HoaDons",
                c => new
                    {
                        MaHD = c.Int(nullable: false, identity: true),
                        MaKH = c.Int(nullable: false),
                        NgayLap = c.DateTime(),
                        TongTien = c.Double(),
                        MaTT = c.Int(),
                        MaNV = c.Int(),
                        MaKM = c.Int(),
                        DiaChiGiaoHang = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.MaHD)
                .ForeignKey("dbo.KhachHangs", t => t.MaKH, cascadeDelete: true)
                .ForeignKey("dbo.KhuyenMais", t => t.MaKM)
                .ForeignKey("dbo.NhanViens", t => t.MaNV)
                .ForeignKey("dbo.TinhTrangs", t => t.MaTT)
                .Index(t => t.MaKH)
                .Index(t => t.MaTT)
                .Index(t => t.MaNV)
                .Index(t => t.MaKM);
            
            CreateTable(
                "dbo.KhachHangs",
                c => new
                    {
                        MaKH = c.Int(nullable: false, identity: true),
                        HoTen = c.String(nullable: false, maxLength: 100),
                        Email = c.String(maxLength: 100),
                        MatKhau = c.String(nullable: false, maxLength: 50),
                        GioiTinh = c.String(maxLength: 4),
                        NgaySinh = c.DateTime(),
                        NgayDangKy = c.DateTime(),
                        SDT = c.String(maxLength: 15),
                        DiaChi = c.String(maxLength: 200),
                        SoDu = c.Double(),
                        UserName = c.String(maxLength: 50),
                        Avartar = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.MaKH);
            
            CreateTable(
                "dbo.KhuyenMais",
                c => new
                    {
                        MaKM = c.Int(nullable: false, identity: true),
                        TenKM = c.String(nullable: false, maxLength: 100),
                        NgayBD = c.DateTime(nullable: false),
                        NgayKT = c.DateTime(nullable: false),
                        PhanTram = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.MaKM);
            
            CreateTable(
                "dbo.NhanViens",
                c => new
                    {
                        MaNV = c.Int(nullable: false, identity: true),
                        HoTen = c.String(nullable: false, maxLength: 100),
                        GioiTinh = c.String(maxLength: 4),
                        NgaySinh = c.DateTime(),
                    })
                .PrimaryKey(t => t.MaNV);
            
            CreateTable(
                "dbo.TinhTrangs",
                c => new
                    {
                        MaTT = c.Int(nullable: false, identity: true),
                        TinhTrang = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.MaTT);
            
            CreateTable(
                "dbo.Mons",
                c => new
                    {
                        MaMon = c.Int(nullable: false, identity: true),
                        TenMon = c.String(nullable: false, maxLength: 100),
                        GiaBan = c.Double(nullable: false),
                        MoTa = c.String(maxLength: 255),
                        HinhAnh = c.String(maxLength: 255),
                        MaLoai = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MaMon)
                .ForeignKey("dbo.Loais", t => t.MaLoai, cascadeDelete: true)
                .Index(t => t.MaLoai);
            
            CreateTable(
                "dbo.Loais",
                c => new
                    {
                        MaLoai = c.Int(nullable: false, identity: true),
                        TenLoai = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.MaLoai);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Mons", "MaLoai", "dbo.Loais");
            DropForeignKey("dbo.ChiTietHoaDons", "MaMon", "dbo.Mons");
            DropForeignKey("dbo.HoaDons", "MaTT", "dbo.TinhTrangs");
            DropForeignKey("dbo.HoaDons", "MaNV", "dbo.NhanViens");
            DropForeignKey("dbo.HoaDons", "MaKM", "dbo.KhuyenMais");
            DropForeignKey("dbo.HoaDons", "MaKH", "dbo.KhachHangs");
            DropForeignKey("dbo.ChiTietHoaDons", "MaHD", "dbo.HoaDons");
            DropIndex("dbo.Mons", new[] { "MaLoai" });
            DropIndex("dbo.HoaDons", new[] { "MaKM" });
            DropIndex("dbo.HoaDons", new[] { "MaNV" });
            DropIndex("dbo.HoaDons", new[] { "MaTT" });
            DropIndex("dbo.HoaDons", new[] { "MaKH" });
            DropIndex("dbo.ChiTietHoaDons", new[] { "MaMon" });
            DropIndex("dbo.ChiTietHoaDons", new[] { "MaHD" });
            DropTable("dbo.Loais");
            DropTable("dbo.Mons");
            DropTable("dbo.TinhTrangs");
            DropTable("dbo.NhanViens");
            DropTable("dbo.KhuyenMais");
            DropTable("dbo.KhachHangs");
            DropTable("dbo.HoaDons");
            DropTable("dbo.ChiTietHoaDons");
            DropTable("dbo.Admins");
        }
    }
}
