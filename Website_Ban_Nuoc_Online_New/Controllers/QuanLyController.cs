using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Website_Ban_Nuoc_Online.Models;

namespace Website_Ban_Nuoc_Online.Controllers
{
    public class QuanLyController : Controller
    {
        // DÙNG ĐÚNG DB CONTEXT CỦA PROJECT
        private readonly WebsiteBanNuocContext db = new WebsiteBanNuocContext();

        // CHẶN TẤT CẢ ACTION – PHẢI LÀ ADMIN MỚI VÀO ĐƯỢC
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Nếu không phải admin -> đá về trang đăng nhập
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                filterContext.Result = RedirectToAction("Login", "User");
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        // ================= DASHBOARD =================
        public ActionResult Index()
        {
            DateTime today = DateTime.Today;
            DateTime weekAgo = today.AddDays(-6); // 7 ngày gần nhất

            // Doanh thu hôm nay
            var doanhThuHomNay = db.HoaDons
                .Where(h => h.NgayLap >= today)
                .Select(h => h.TongTien)
                .DefaultIfEmpty(0)
                .Sum();

            // Số đơn hôm nay
            var donHangHomNay = db.HoaDons.Count(h => h.NgayLap >= today);

            // Số khách mới hôm nay
            var khachMoiHomNay = db.KhachHangs.Count(k => k.NgayDangKy >= today);

            // Doanh thu 7 ngày gần nhất
            var doanhThuTuan = db.HoaDons
                .Where(h => h.NgayLap >= weekAgo)
                .AsEnumerable() // chuyển sang LINQ to Objects để group theo Date
                .GroupBy(h => h.NgayLap.Value.Date)
                .Select(g => new
                {
                    Ngay = g.Key,
                    TongTien = g.Sum(x => Convert.ToDecimal(x.TongTien))
                })
                .OrderBy(g => g.Ngay)
                .ToList();

            var ngayTrongTuan = Enumerable.Range(0, 7)
                .Select(i => today.AddDays(-i))
                .OrderBy(d => d)
                .ToList();

            var duLieuTuan = ngayTrongTuan.Select(d => new
            {
                Ngay = d.ToString("dd/MM"),
                DoanhThu = doanhThuTuan.FirstOrDefault(x => x.Ngay.Date == d.Date)?.TongTien ?? 0
            }).ToList();

            ViewBag.DoanhThuHomNay = doanhThuHomNay;
            ViewBag.DonHangHomNay = donHangHomNay;
            ViewBag.KhachMoiHomNay = khachMoiHomNay;
            ViewBag.NgayTuan = duLieuTuan.Select(x => x.Ngay).ToArray();
            ViewBag.DoanhThuTuan = duLieuTuan.Select(x => x.DoanhThu).ToArray();

            return View();
        }

        // ================= ĐƠN HÀNG =================
        public ActionResult OrderManage(string search, string status, DateTime? date, int page = 1)
        {
            var orders = db.HoaDons
                .Include(h => h.KhachHang)
                .Include(h => h.TinhTrang)
                .Include(h => h.ChiTietHoaDons.Select(ct => ct.Mon))
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                orders = orders.Where(h => h.KhachHang.HoTen.Contains(search));
            }
            if (!string.IsNullOrEmpty(status) && status != "Tất cả")
            {
                orders = orders.Where(h => h.TinhTrang.TinhTrangDon == status);
                // nếu property tên khác (vd: TenTrangThai) thì sửa lại chỗ này
            }
            if (date.HasValue)
            {
                DateTime d = date.Value.Date;
                DateTime dNext = d.AddDays(1);
                orders = orders.Where(h => h.NgayLap >= d && h.NgayLap < dNext);
            }

            int pageSize = 5;
            int total = orders.Count();
            int noOfPages = (int)Math.Ceiling((double)total / pageSize);
            int skip = (page - 1) * pageSize;

            var list = orders
                .OrderByDescending(h => h.NgayLap)
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            ViewBag.Page = page;
            ViewBag.NoOfPages = noOfPages;

            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CapNhatTrangThai(int id, string status)
        {
            try
            {
                var hoaDon = db.HoaDons.Find(id);
                if (hoaDon == null)
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng." });

                if (string.IsNullOrWhiteSpace(status))
                {
                    hoaDon.MaTT = null;
                    db.SaveChanges();
                    return Json(new { success = true });
                }

                var tinhTrang = db.TinhTrangs.FirstOrDefault(t => t.TinhTrangDon == status);
                if (tinhTrang == null)
                {
                    tinhTrang = new TinhTrang { TinhTrangDon = status };
                    db.TinhTrangs.Add(tinhTrang);
                    db.SaveChanges();
                }

                hoaDon.MaTT = tinhTrang.MaTT;
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ================= KHÁCH HÀNG =================
        public ActionResult Customer(string keyword, int page = 1)
        {
            var khachHang = db.KhachHangs.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                var k = keyword.ToLower().Trim();
                khachHang = khachHang.Where(x => x.HoTen.ToLower().Contains(k));
            }

            int pageSize = 5;
            int total = khachHang.Count();
            int noOfPages = (int)Math.Ceiling((double)total / pageSize);
            int skip = (page - 1) * pageSize;

            var list = khachHang
                .OrderBy(x => x.HoTen)
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            ViewBag.Page = page;
            ViewBag.NoOfPages = noOfPages;

            return View(list);
        }

        // ================= SẢN PHẨM =================
        public ActionResult SanPham(string keyword, int page = 1, int? maLoai = null)
        {
            ViewBag.MaLoai = new SelectList(db.Loais.OrderBy(l => l.TenLoai).ToList(), "MaLoai", "TenLoai", maLoai);
            ViewBag.Loais = db.Loais.OrderBy(l => l.TenLoai).ToList();

            IEnumerable<Mon> allItems;

            if (maLoai.HasValue && maLoai.Value > 0)
            {
                var p = new System.Data.SqlClient.SqlParameter("@MaLoai", maLoai.Value);
                allItems = db.Database.SqlQuery<Mon>("SELECT * FROM dbo.Lay_DS_Mon(@MaLoai)", p).ToList();
            }
            else
            {
                allItems = db.Mons.ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                var k = keyword.Trim().ToLower();
                allItems = allItems.Where(x => (x.TenMon ?? "").ToLower().Contains(k)).ToList();
            }

            int pageSize = 5;
            int total = allItems.Count();
            int noOfPages = (int)Math.Ceiling((double)total / pageSize);
            if (page < 1) page = 1;
            if (page > noOfPages && noOfPages > 0) page = noOfPages;

            var list = allItems
                .OrderBy(x => x.TenMon)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Page = page;
            ViewBag.NoOfPages = noOfPages;
            ViewBag.Keyword = keyword;
            ViewBag.SelectedMaLoai = maLoai;

            return View(list);
        }

        // GET: QuanLy/Create
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.MaLoai = new SelectList(db.Loais.OrderBy(l => l.TenLoai).ToList(), "MaLoai", "TenLoai");
            return View();
        }

        // POST: QuanLy/Create (gọi procedure Them_Mon)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Mon mon, HttpPostedFileBase HinhAnhFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MaLoai = new SelectList(db.Loais, "MaLoai", "TenLoai", mon.MaLoai);
                return View(mon);
            }

            if (mon.GiaBan <= 0)
            {
                ModelState.AddModelError("GiaBan", "Giá bán phải lớn hơn 0.");
                ViewBag.MaLoai = new SelectList(db.Loais, "MaLoai", "TenLoai", mon.MaLoai);
                return View(mon);
            }

            string savedFilePath = null;
            try
            {
                if (HinhAnhFile != null && HinhAnhFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(HinhAnhFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                    HinhAnhFile.SaveAs(path);
                    mon.HinhAnh = fileName;
                    savedFilePath = path;
                }

                var pTen = new System.Data.SqlClient.SqlParameter("@TenMon", (object)mon.TenMon ?? DBNull.Value);
                var pGia = new System.Data.SqlClient.SqlParameter("@GiaBan", mon.GiaBan);
                var pMoTa = new System.Data.SqlClient.SqlParameter("@MoTa", (object)mon.MoTa ?? DBNull.Value);
                var pHinh = new System.Data.SqlClient.SqlParameter("@HinhAnh", (object)mon.HinhAnh ?? DBNull.Value);
                var pMaLoai = new System.Data.SqlClient.SqlParameter("@MaLoai", mon.MaLoai);

                db.Database.ExecuteSqlCommand(
                    "EXEC Them_Mon @TenMon, @GiaBan, @MoTa, @HinhAnh, @MaLoai",
                    pTen, pGia, pMoTa, pHinh, pMaLoai
                );

                TempData["SuccessMessage"] = "Thêm món bằng stored procedure thành công.";
                return RedirectToAction("SanPham");
            }
            catch (System.Data.SqlClient.SqlException sqlEx)
            {
                ModelState.AddModelError("", "Lỗi cơ sở dữ liệu: " + sqlEx.Message);

                if (!string.IsNullOrEmpty(savedFilePath) && System.IO.File.Exists(savedFilePath))
                {
                    try { System.IO.File.Delete(savedFilePath); } catch { }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi: " + ex.Message);
                if (!string.IsNullOrEmpty(savedFilePath) && System.IO.File.Exists(savedFilePath))
                {
                    try { System.IO.File.Delete(savedFilePath); } catch { }
                }
            }

            ViewBag.MaLoai = new SelectList(db.Loais, "MaLoai", "TenLoai", mon.MaLoai);
            return View(mon);
        }

        // GET: QuanLy/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var mon = db.Mons.Find(id);
            if (mon == null)
                return HttpNotFound();

            ViewBag.MaLoai = new SelectList(db.Loais, "MaLoai", "TenLoai", mon.MaLoai);
            return View(mon);
        }

        // POST: QuanLy/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Mon mon, HttpPostedFileBase HinhAnhFile)
        {
            if (ModelState.IsValid)
            {
                if (HinhAnhFile != null && HinhAnhFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(HinhAnhFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                    HinhAnhFile.SaveAs(path);
                    mon.HinhAnh = fileName;
                }

                db.Entry(mon).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SanPham");
            }

            ViewBag.MaLoai = new SelectList(db.Loais, "MaLoai", "TenLoai", mon.MaLoai);
            return View(mon);
        }

        // GET: QuanLy/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var mon = db.Mons.Find(id);
            if (mon == null)
                return HttpNotFound();

            return View(mon);
        }

        // POST: QuanLy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var mon = db.Mons.Find(id);
            if (mon == null) return RedirectToAction("SanPham");

            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    var details = db.ChiTietHoaDons.Where(ct => ct.MaMon == id).ToList();
                    if (details.Any())
                        db.ChiTietHoaDons.RemoveRange(details);

                    db.Mons.Remove(mon);
                    db.SaveChanges();

                    tran.Commit();
                    TempData["SuccessMessage"] = "Xóa món và các chi tiết liên quan thành công.";
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    TempData["ErrorMessage"] = "Lỗi khi xóa: " + ex.Message;
                }
            }

            return RedirectToAction("SanPham");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteOrder(int id)
        {
            try
            {
                var order = db.HoaDons
                    .Include(h => h.ChiTietHoaDons)
                    .FirstOrDefault(x => x.MaHD == id);
                if (order == null)
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng." });

                if (order.ChiTietHoaDons.Any())
                    db.ChiTietHoaDons.RemoveRange(order.ChiTietHoaDons);

                db.HoaDons.Remove(order);
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ================= CỘNG TIỀN VÍ =================
        public ActionResult CongTien()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CongTien(string Username, double SoTien)
        {
            if (string.IsNullOrEmpty(Username) || SoTien <= 0)
            {
                ViewBag.Error = "Vui lòng nhập tên đăng nhập và số tiền hợp lệ!";
                return View();
            }

            var kh = db.KhachHangs.FirstOrDefault(k => k.UserName == Username);
            if (kh == null)
            {
                ViewBag.Error = "Không tìm thấy khách hàng với tên đăng nhập này!";
                return View();
            }

            if (kh.SoDu == null) kh.SoDu = 0;
            kh.SoDu += SoTien;
            db.SaveChanges();

            ViewBag.Success = $"Đã cộng {SoTien:N0} VNĐ vào tài khoản của {kh.HoTen} (User: {kh.UserName}).";
            return View();
        }

        // ================= NHÂN VIÊN + GIAO ĐƠN =================
        public ActionResult Staff()
        {
            var danhSachNhanVien = db.NhanViens.OrderBy(n => n.HoTen).ToList();

            var trangThaiDangXuLy = db.TinhTrangs.FirstOrDefault(t => t.TinhTrangDon == "Đang xử lý");
            int pendingCount = 0;

            if (trangThaiDangXuLy != null)
                pendingCount = db.HoaDons.Count(h => h.MaTT == trangThaiDangXuLy.MaTT);
            else
                pendingCount = db.HoaDons.Count(h => h.MaTT == null);

            ViewBag.PendingOrdersCount = pendingCount;
            return View(danhSachNhanVien);
        }

        public ActionResult NhanDon()
        {
            var trangThaiDangXuLy = db.TinhTrangs.FirstOrDefault(t => t.TinhTrangDon == "Đang xử lý");
            var danhSachDon = new List<HoaDon>();

            if (trangThaiDangXuLy != null)
            {
                danhSachDon = db.HoaDons
                    .Include(h => h.KhachHang)
                    .Include(h => h.TinhTrang)
                    .Include(h => h.ChiTietHoaDons.Select(ct => ct.Mon))
                    .Where(h => h.MaTT == trangThaiDangXuLy.MaTT)
                    .OrderByDescending(h => h.NgayLap)
                    .ToList();
            }

            return View(danhSachDon);
        }

        public ActionResult NhanTinhTrang()
        {
            var ids = Session["ToShipperIds"] as List<int>;
            if (ids == null || !ids.Any())
                return View(new List<HoaDon>());

            var danhSach = db.HoaDons
                .Include(h => h.KhachHang)
                .Include(h => h.ChiTietHoaDons.Select(ct => ct.Mon))
                .Where(h => ids.Contains(h.MaHD))
                .OrderByDescending(h => h.NgayLap)
                .ToList();

            return View(danhSach);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NhanDonNhanVien(int id)
        {
            var hd = db.HoaDons
                .Include(h => h.KhachHang)
                .Include(h => h.ChiTietHoaDons.Select(ct => ct.Mon))
                .FirstOrDefault(h => h.MaHD == id);

            if (hd == null)
            {
                TempData["Error"] = "Không tìm thấy hóa đơn.";
                return RedirectToAction("NhanDon");
            }

            var list = Session["ToShipperIds"] as List<int> ?? new List<int>();
            if (!list.Contains(id))
            {
                list.Add(id);
                Session["ToShipperIds"] = list;
            }

            TempData["Success"] = "Đơn hàng được giao thành công!";
            return RedirectToAction("NhanVien", new { id });
        }

        public ActionResult DanhSachDonNhanVien()
        {
            var ids = Session["ToShipperIds"] as List<int>;
            if (ids == null || !ids.Any())
                return View(new List<HoaDon>());

            var danhSach = db.HoaDons
                .Include(h => h.KhachHang)
                .Include(h => h.ChiTietHoaDons.Select(ct => ct.Mon))
                .Where(h => ids.Contains(h.MaHD))
                .OrderByDescending(h => h.NgayLap)
                .ToList();

            return View(danhSach);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XacNhanTuShipper(int id)
        {
            var hd = db.HoaDons.FirstOrDefault(h => h.MaHD == id);
            if (hd == null)
            {
                TempData["Error"] = "Không tìm thấy hóa đơn.";
                return RedirectToAction("NhanTinhTrang");
            }

            try
            {
                var ttHoanThanh = db.TinhTrangs.FirstOrDefault(t => t.TinhTrangDon == "Hoàn thành");
                if (ttHoanThanh == null)
                {
                    ttHoanThanh = new TinhTrang { TinhTrangDon = "Hoàn thành" };
                    db.TinhTrangs.Add(ttHoanThanh);
                    db.SaveChanges();
                }

                hd.MaTT = ttHoanThanh.MaTT;
                db.Entry(hd).State = EntityState.Modified;
                db.SaveChanges();

                var list = Session["ToShipperIds"] as List<int>;
                if (list != null && list.Contains(id))
                {
                    list.Remove(id);
                    Session["ToShipperIds"] = list;
                }

                TempData["Success"] = $"Xác nhận hoàn thành hóa đơn #{id} thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xác nhận: " + ex.Message;
            }

            return RedirectToAction("NhanTinhTrang");
        }

        // ================= DISPOSE =================
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }// GIẢM GIÁ NƯỚC ÉP (Gọi stored procedure GiamGiaNuocEp)
        [HttpGet]
        public ActionResult ApplyDiscount()
        {
            try
            {
                // Gọi stored procedure
                db.Database.ExecuteSqlCommand("EXEC GiamGiaNuocEp");

                TempData["SuccessMessage"] = "Hoàn tất giảm giá các món Nước Ép trong tháng khuyến mãi!";
            }
            catch (System.Data.SqlClient.SqlException)
            {
                // Ví dụ: SP tự RAISEERROR khi hôm nay không thuộc tháng 10, 11
                TempData["ErrorMessage"] = "Hôm nay không thuộc tháng 10 hoặc 11 — không áp dụng giảm giá.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
            }

            // Sau khi chạy xong quay về trang quản lý sản phẩm
            return RedirectToAction("SanPham");
        }

    }
}
