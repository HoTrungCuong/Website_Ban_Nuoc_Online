using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Website_Ban_Nuoc_Online.Models
{
    public class ChiTietHoaDon
    {
        public int MaHD { get; set; }

        public int MaMon { get; set; }

        public int SoLuong { get; set; }

        public double GiaBan { get; set; }

        // Navigation
        [ForeignKey("MaHD")]
        public virtual HoaDon HoaDon { get; set; }

        [ForeignKey("MaMon")]
        public virtual Mon Mon { get; set; }
    }
}