using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Website_Ban_Nuoc_Online.Models
{
    public class HoaDon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaHD { get; set; }

        public int MaKH { get; set; }

        public DateTime? NgayLap { get; set; }

        public double? TongTien { get; set; }

        public int? MaTT { get; set; }

        public int? MaNV { get; set; }

        public int? MaKM { get; set; }

        [StringLength(200)]
        public string DiaChiGiaoHang { get; set; }

        // Navigation
        [ForeignKey("MaKH")]
        public virtual KhachHang KhachHang { get; set; }

        [ForeignKey("MaTT")]
        public virtual TinhTrang TinhTrang { get; set; }

        [ForeignKey("MaNV")]
        public virtual NhanVien NhanVien { get; set; }

        [ForeignKey("MaKM")]
        public virtual KhuyenMai KhuyenMai { get; set; }

        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }

        public HoaDon()
        {
            ChiTietHoaDons = new HashSet<ChiTietHoaDon>();
        }
    }
}