using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Website_Ban_Nuoc_Online.Models
{
    public class KhachHang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaKH { get; set; }

        [Required, StringLength(100)]
        public string HoTen { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [Required, StringLength(50)]
        public string MatKhau { get; set; }

        [StringLength(4)]
        public string GioiTinh { get; set; }   // Nam / Nữ

        public DateTime? NgaySinh { get; set; }

        public DateTime? NgayDangKy { get; set; }

        [StringLength(15)]
        public string SDT { get; set; }

        [StringLength(200)]
        public string DiaChi { get; set; }

        public double? SoDu { get; set; }

        [StringLength(50)]
        public string UserName { get; set; }

        [StringLength(255)]
        public string Avartar { get; set; }

        // Navigation
        public virtual ICollection<HoaDon> HoaDons { get; set; }

        public KhachHang()
        {
            HoaDons = new HashSet<HoaDon>();
        }

    }
}