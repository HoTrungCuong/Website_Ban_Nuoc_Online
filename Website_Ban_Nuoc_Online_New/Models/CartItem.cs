using System;
using System.Linq;

namespace Website_Ban_Nuoc_Online.Models
{
    [Serializable]
    public class CartItem
    {
        public int iMaMon { get; set; }
        public string sTenMon { get; set; }
        public string sHinhAnh { get; set; }
        public float dDonGia { get; set; }
        public int iSoLuong { get; set; }

        public float ThanhTien
        {
            get { return iSoLuong * dDonGia; }
        }

        // Dùng đúng DbContext của project hiện tại
        WebsiteBanNuocContext db = new WebsiteBanNuocContext();

        public CartItem(int Masp)
        {
            // Nên dùng SingleOrDefault để tránh throw nếu không có món
            Mon sp = db.Mons.SingleOrDefault(n => n.MaMon == Masp);
            if (sp != null)
            {
                iMaMon = sp.MaMon;
                sTenMon = sp.TenMon;
                sHinhAnh = sp.HinhAnh;
                dDonGia = (float)sp.GiaBan;   // hoặc (float) nếu GiaBan là decimal
                iSoLuong = 1;
            }
        }
    }
}
