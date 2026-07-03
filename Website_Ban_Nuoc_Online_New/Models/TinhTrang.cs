using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Website_Ban_Nuoc_Online.Models
{
    public class TinhTrang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaTT { get; set; }

        [Required, StringLength(50)]
        public string TinhTrangDon { get; set; }   // Đổi tên property để khỏi trùng với class

        // Navigation
        public virtual ICollection<HoaDon> HoaDons { get; set; }

        public TinhTrang()
        {
            HoaDons = new HashSet<HoaDon>();
        }
    }
}