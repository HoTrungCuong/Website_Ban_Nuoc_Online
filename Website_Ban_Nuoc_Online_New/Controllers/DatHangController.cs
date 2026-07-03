using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Website_Ban_Nuoc_Online.Models;

namespace Website_Ban_Nuoc_Online.Controllers
{
    public class DatHangController : Controller
    {
        private WebsiteBanNuocContext db = new WebsiteBanNuocContext();

        // 🏠 Trang chính đặt hàng
        public ActionResult Index()
        {
            return View();
        }

        // ➕ Thêm món vào giỏ hàng
        public ActionResult ThemMatHang(int msp, int soluong)
        {
            var gh = Session["gh"] as GioHang ?? new GioHang();
            gh.Them(msp, soluong);

            Session["gh"] = gh;
            Session["SoLuongGioHang"] = gh.TongSLHang();
            return RedirectToAction("Index", "Mons");
        }

        // 🛒 Xem giỏ hàng
        public ActionResult XemGioHang()
        {
            var gh = Session["gh"] as GioHang ?? new GioHang();

            var dsKM = db.KhuyenMais
                .Where(x => x.NgayBD <= DateTime.Now && x.NgayKT >= DateTime.Now)
                .ToList();

            ViewBag.MaGiamGiaList = dsKM;
            return View(gh);
        }

        // 🎁 Áp dụng mã giảm giá
        [HttpPost]
        public ActionResult ApDungMaGiamGia(int maGiamGia)
        {
            var gh = Session["gh"] as GioHang;

            if (gh == null || gh.lst == null || !gh.lst.Any())
            {
                TempData["ThongBao"] = "Giỏ hàng trống!";
                return RedirectToAction("XemGioHang");
            }

            var km = db.KhuyenMais
                .FirstOrDefault(x => x.MaKM == maGiamGia &&
                                     x.NgayBD <= DateTime.Now &&
                                     x.NgayKT >= DateTime.Now);

            if (km != null)
            {
                gh.PhanTramGiam = (float)km.PhanTram;
                gh.TenKM = km.TenKM;
                TempData["ThongBao"] = $"Áp dụng mã '{km.TenKM}' giảm {km.PhanTram}% thành công!";
            }
            else
            {
                gh.PhanTramGiam = 0;
                gh.TenKM = null;
                TempData["ThongBao"] = "Mã giảm giá không hợp lệ hoặc không tồn tại.";
            }

            Session["gh"] = gh;
            return RedirectToAction("XemGioHang");
        }

        // ❌ Xóa món
        public ActionResult XoaMatHang(int msp)
        {
            var gh = Session["gh"] as GioHang ?? new GioHang();
            gh.Xoa(msp);

            Session["gh"] = gh;
            Session["SoLuongGioHang"] = gh.TongSLHang();
            return RedirectToAction("XemGioHang");
        }

        // ------------------------------------------
        //              XÁC NHẬN ĐƠN HÀNG
        // ------------------------------------------
        public ActionResult XacNhanDonHang()
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "User");

            var user = (KhachHang)Session["User"];
            var gh = Session["gh"] as GioHang;

            if (gh == null || gh.lst == null || !gh.lst.Any())
                return RedirectToAction("Index", "Mons");

            ViewBag.KhachHang = user;
            ViewBag.TenKM = gh.TenKM;
            ViewBag.PhanTramGiam = gh.PhanTramGiam;

            return View(gh);
        }

        // ------------------------------------------
        //              THANH TOÁN
        // ------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThanhToan(string PhuongThucTT, string PhuongThucThanhToan)
        {
            // PhuongThucTT: name mới trên view (MoMo, Banking, COD)
            // PhuongThucThanhToan: name cũ (Vi, TienMat) – cho chắc ăn, mình lấy cái nào có giá trị
            var method = !string.IsNullOrEmpty(PhuongThucTT)
                ? PhuongThucTT
                : PhuongThucThanhToan;

            const decimal PHI_VAN_CHUYEN = 15000m;
            var cart = Session["gh"] as GioHang;

            if (cart == null || cart.lst == null || !cart.lst.Any())
            {
                TempData["Error"] = "Giỏ hàng rỗng.";
                return RedirectToAction("XemGioHang");
            }

            if (Session["MaKH"] == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập.";
                return RedirectToAction("Login", "User");
            }

            int maKH = Convert.ToInt32(Session["MaKH"]);
            var kh = db.KhachHangs.Find(maKH);

            if (kh == null)
            {
                TempData["Error"] = "Không tìm thấy khách hàng.";
                return RedirectToAction("XemGioHang");
            }

            if (string.IsNullOrEmpty(method))
            {
                TempData["Error"] = "Vui lòng chọn phương thức thanh toán.";
                return RedirectToAction("XacNhanDonHang");
            }

            // Tổng tiền hàng sau khi giảm giá (chưa gồm phí ship)
            decimal tongHang = Convert.ToDecimal(cart.TongThanhTienSauGiam());
            decimal tongThanhToan = tongHang + PHI_VAN_CHUYEN;

            // 🔥 LẤY / TẠO TRẠNG THÁI "Đang xử lý"
            var ttXuLy = db.TinhTrangs.FirstOrDefault(t => t.TinhTrangDon == "Đang xử lý");
            if (ttXuLy == null)
            {
                ttXuLy = new TinhTrang { TinhTrangDon = "Đang xử lý" };
                db.TinhTrangs.Add(ttXuLy);
                db.SaveChanges();
            }

            // ================== TẠO HÓA ĐƠN CHUNG CHO MỌI PHƯƠNG THỨC ==================
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    var hd = new HoaDon
                    {
                        MaKH = maKH,
                        NgayLap = DateTime.Now,
                        TongTien = (double)tongThanhToan,
                        MaTT = ttXuLy.MaTT,
                        DiaChiGiaoHang = kh.DiaChi
                        // Nếu HoaDon có cột lưu phương thức thanh toán, bạn gán ở đây
                        // Ví dụ: PhuongThucTT = method
                    };

                    // Có mã KM thì gắn vào
                    if (!string.IsNullOrEmpty(cart.TenKM))
                    {
                        var km = db.KhuyenMais.FirstOrDefault(x => x.TenKM == cart.TenKM);
                        if (km != null) hd.MaKM = km.MaKM;
                    }

                    db.HoaDons.Add(hd);
                    db.SaveChanges();

                    // Chi tiết hóa đơn
                    foreach (var it in cart.lst)
                    {
                        db.ChiTietHoaDons.Add(new ChiTietHoaDon
                        {
                            MaHD = hd.MaHD,
                            MaMon = it.iMaMon,
                            SoLuong = it.iSoLuong,
                            GiaBan = (double)it.dDonGia
                        });
                    }
                    db.SaveChanges();

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    TempData["Error"] = "Lỗi khi tạo đơn hàng: " + ex.Message;
                    return RedirectToAction("XacNhanDonHang");
                }
            }

            // XÓA GIỎ HÀNG
            Session.Remove("gh");
            Session["SoLuongGioHang"] = 0;

            // Thông báo tùy theo phương thức
            switch (method)
            {
                case "MoMo":
                    TempData["ThongBao"] =
                        "Đặt hàng thành công! Vui lòng thanh toán qua ví MoMo theo hướng dẫn của cửa hàng.";
                    break;

                case "Banking":
                    TempData["ThongBao"] =
                        "Đặt hàng thành công! Vui lòng chuyển khoản ngân hàng theo thông tin cửa hàng cung cấp.";
                    break;

                case "COD":
                default:
                    TempData["ThongBao"] =
                        "Đặt hàng thành công! Bạn sẽ thanh toán khi nhận hàng (COD).";
                    break;
            }

            return RedirectToAction("Index", "Home");
        }
    }
}