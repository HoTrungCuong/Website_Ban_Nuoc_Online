using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Website_Ban_Nuoc_Online.Models
{
    public class KhuyenMai
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaKM { get; set; }

        [Required, StringLength(100)]
        public string TenKM { get; set; }

        public DateTime NgayBD { get; set; }
        public DateTime NgayKT { get; set; }

        public double PhanTram { get; set; }

        // Navigation
        public virtual ICollection<HoaDon> HoaDons { get; set; }

        public KhuyenMai()
        {
            HoaDons = new HashSet<HoaDon>();
        }
    }
}