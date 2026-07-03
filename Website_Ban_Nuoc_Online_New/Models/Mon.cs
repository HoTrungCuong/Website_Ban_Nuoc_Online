using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
namespace Website_Ban_Nuoc_Online.Models
{
    public class Mon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaMon { get; set; }

        [Required, StringLength(100)]
        public string TenMon { get; set; }

        public double GiaBan { get; set; }

        [StringLength(255)]
        public string MoTa { get; set; }

        [StringLength(255)]
        public string HinhAnh { get; set; }

        public int MaLoai { get; set; }

        // Navigation
        [ForeignKey("MaLoai")]
        public virtual Loai Loai { get; set; }

        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }

        public Mon()
        {
            ChiTietHoaDons = new HashSet<ChiTietHoaDon>();
        }
    }
}