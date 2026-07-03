using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website_Ban_Nuoc_Online.Models
{
    [Serializable]
    public class GioHang
    {
        public List<CartItem> lst { get; set; }

        public GioHang()
        {
            lst = new List<CartItem>();
        }

        public GioHang(List<CartItem> lstGH)
        {
            lst = lstGH;
        }

        // Thông tin khuyến mãi
        public string TenKM { get; set; }
        public float PhanTramGiam { get; set; }

        public float TongThanhTienSauGiam()
        {
            var tong = TongThanhTien();
            if (PhanTramGiam > 0)
            {
                return tong * (1 - PhanTramGiam / 100);
            }
            return tong;
        }

        public int SoMatHang()
        {
            return lst.Count;
        }

        public int TongSLHang()
        {
            return lst.Sum(n => n.iSoLuong);
        }

        public float TongThanhTien()
        {
            return lst.Sum(n => n.ThanhTien);
        }

        public int Them(int iMaSP, int soluong = 1)
        {
            CartItem sanpham = lst.Find(n => n.iMaMon == iMaSP);

            if (sanpham == null)
            {
                CartItem sp = new CartItem(iMaSP);
                if (sp == null)
                    return -1;

                sp.iSoLuong = soluong;
                lst.Add(sp);
            }
            else
            {
                sanpham.iSoLuong += soluong;
            }

            return 1;
        }

        public void Xoa(int msp)
        {
            var sp = lst.FirstOrDefault(x => x.iMaMon == msp);
            if (sp != null)
                lst.Remove(sp);
        }

        public void CapNhat(int msp, int soLuong)
        {
            var sp = lst.FirstOrDefault(x => x.iMaMon == msp);
            if (sp != null)
                sp.iSoLuong = soLuong;
        }
    }
}
