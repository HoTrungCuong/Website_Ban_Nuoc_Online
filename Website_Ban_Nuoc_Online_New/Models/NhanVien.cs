using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Website_Ban_Nuoc_Online.Models
{
    public class NhanVien
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaNV { get; set; }

        [Required, StringLength(100)]
        public string HoTen { get; set; }

        [StringLength(4)]
        public string GioiTinh { get; set; }

        public DateTime? NgaySinh { get; set; }

        // Navigation
        public virtual ICollection<HoaDon> HoaDons { get; set; }

        public NhanVien()
        {
            HoaDons = new HashSet<HoaDon>();
        }
    }
}