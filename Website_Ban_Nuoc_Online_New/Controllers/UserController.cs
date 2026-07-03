using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Website_Ban_Nuoc_Online.Models;

namespace Website_Ban_Nuoc_Online.Controllers
{
    public class UserController : Controller
    {
        private WebsiteBanNuocContext db = new WebsiteBanNuocContext();

        // GET: User/Login
        public ActionResult Login()
        {
            return View();
        }

        // GET: User/DangKy
        public ActionResult DangKy()
        {
            // View Login chứa cả 2 tab, bật sẵn tab Đăng ký
            ViewBag.ShowRegister = true;
            return View("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy(KhachHang kh, string XacNhanMatKhau)
        {
            ViewBag.ShowRegister = true;

            // 1) Validate các trường bắt buộc
            if (string.IsNullOrWhiteSpace(kh.UserName))
                ModelState.AddModelError(nameof(kh.UserName), "Nhập thiếu tên đăng nhập");

            if (string.IsNullOrWhiteSpace(kh.HoTen))
                ModelState.AddModelError(nameof(kh.HoTen), "Nhập thiếu họ tên");

            if (string.IsNullOrWhiteSpace(kh.Email))
                ModelState.AddModelError(nameof(kh.Email), "Nhập thiếu email");

            if (string.IsNullOrWhiteSpace(kh.MatKhau))
                ModelState.AddModelError(nameof(kh.MatKhau), "Nhập thiếu mật khẩu");

            if (string.IsNullOrWhiteSpace(XacNhanMatKhau))
                ModelState.AddModelError("XacNhanMatKhau", "Vui lòng xác nhận mật khẩu");

            if (string.IsNullOrWhiteSpace(kh.GioiTinh))
                ModelState.AddModelError(nameof(kh.GioiTinh), "Vui lòng chọn giới tính");

            if (string.IsNullOrWhiteSpace(kh.SDT))
                ModelState.AddModelError(nameof(kh.SDT), "Nhập thiếu số điện thoại");

            if (string.IsNullOrWhiteSpace(kh.DiaChi))
                ModelState.AddModelError(nameof(kh.DiaChi), "Nhập thiếu địa chỉ giao hàng");

            // 2) Kiểm tra độ mạnh mật khẩu
            var pass = kh.MatKhau ?? string.Empty;
            bool hopLeMatKhau =
                pass.Length >= 6 &&
                Regex.IsMatch(pass, @"[A-Za-z]") &&
                Regex.IsMatch(pass, @"[0-9]");

            if (!hopLeMatKhau)
            {
                ModelState.AddModelError(nameof(kh.MatKhau),
                    "Mật khẩu phải tối thiểu 6 ký tự, gồm cả chữ và số!");
            }

            // 3) Kiểm tra xác nhận mật khẩu
            if (!string.IsNullOrEmpty(kh.MatKhau) && kh.MatKhau != XacNhanMatKhau)
            {
                ModelState.AddModelError("XacNhanMatKhau", "Mật khẩu xác nhận không khớp!");
            }

            // 4) Kiểm tra trùng Username / Email (dùng DbSet KhachHangs)
            if (!string.IsNullOrWhiteSpace(kh.Email) && !string.IsNullOrWhiteSpace(kh.UserName))
            {
                var existingUser = db.KhachHangs
                    .FirstOrDefault(u => u.Email == kh.Email || u.UserName == kh.UserName);

                if (existingUser != null)
                    ModelState.AddModelError("", "Email hoặc tên đăng nhập đã được sử dụng!");
            }

            // Nếu có lỗi -> trả lại view Login (tab đăng ký đang mở)
            if (!ModelState.IsValid)
            {
                return View("Login", kh);
            }

            // Lưu user mới
            kh.Avartar = null;
            kh.NgayDangKy = DateTime.Now;

            db.KhachHangs.Add(kh);
            db.SaveChanges();

            TempData["ThongBao"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login", "User");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string UsernameOrEmail, string Password)
        {
            if (string.IsNullOrWhiteSpace(UsernameOrEmail) || string.IsNullOrWhiteSpace(Password))
            {
                TempData["Error"] = "Vui lòng nhập tên đăng nhập / email và mật khẩu.";
                return RedirectToAction("Login");
            }

            // 1️⃣ Kiểm tra ADMIN trước
            var admin = db.Admins.FirstOrDefault(a =>
                a.Username == UsernameOrEmail && a.Password == Password);

            if (admin != null)
            {
                // Ghi session admin
                Session["IsAdmin"] = true;
                Session["Admin"] = admin;

                // Xóa session user nếu có
                Session["MaKH"] = null;
                Session["HoTen"] = null;
                Session["User"] = null;

                TempData["ThongBao"] = "Đăng nhập quản trị thành công!";
                return RedirectToAction("Index", "QuanLy");   // 👉 nhảy vào trang admin
            }

            // 2️⃣ Không phải admin thì kiểm tra KHÁCH HÀNG
            var user = db.KhachHangs.FirstOrDefault(k =>
                (k.Email == UsernameOrEmail || k.UserName == UsernameOrEmail) &&
                k.MatKhau == Password);

            if (user != null)
            {
                Session["IsAdmin"] = false;      // đánh dấu không phải admin
                Session["MaKH"] = user.MaKH;
                Session["HoTen"] = user.HoTen;
                Session["User"] = user;

                TempData["ThongBao"] = "Đăng nhập thành công!";
                return RedirectToAction("Index", "Home");     // 👉 giao diện user
            }

            // 3️⃣ Sai hết
            TempData["Error"] = "Tài khoản hoặc mật khẩu không đúng!";
            return RedirectToAction("Login");
        }

        // GET logout (link/modal)
        [HttpGet]
        public ActionResult ShowLogout()
        {
            return RedirectToAction("Index", "Home");
        }

        // POST logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            TempData["ThongBao"] = "Bạn đã đăng xuất.";
            return RedirectToAction("Index", "Home");
        }


        public ActionResult LichSuMuaHang()
        {
            if (Session["MaKH"] == null)
                return RedirectToAction("Login", "User");

            int maKH = Convert.ToInt32(Session["MaKH"]);

            var donHangKH = db.HoaDons
                .Where(d => d.MaKH == maKH)
                .OrderByDescending(d => d.NgayLap)
                .ToList();

            return View(donHangKH);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: User/QuenMatKhau
        [HttpGet]
        public ActionResult QuenMatKhau()
        {
            return View();
        }

        // POST: User/QuenMatKhau
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuenMatKhau(string UserName, string EmailOrSDT)
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(EmailOrSDT))
            {
                ViewBag.Error = "Vui lòng nhập tên đăng nhập và email hoặc số điện thoại.";
                return View();
            }

            // Gọi TVF / Stored Function trả về KhachHang
            var user = db.KhachHangs.SqlQuery(
                "SELECT * FROM dbo.KIEMTRA_USERNAME_EMAIL_HOAC_SDT(@UserName, @EmailOrSDT);",
                new System.Data.SqlClient.SqlParameter("@UserName", UserName),
                new System.Data.SqlClient.SqlParameter("@EmailOrSDT", EmailOrSDT)
            ).FirstOrDefault();

            if (user == null)
            {
                ViewBag.Error = "Không tìm thấy tài khoản với thông tin đã cung cấp.";
                return View();
            }

            TempData["EmailOrSDT"] = EmailOrSDT;
            return RedirectToAction("DoiMatKhau");
        }

        // GET: User/DoiMatKhau
        [HttpGet]
        public ActionResult DoiMatKhau()
        {
            if (TempData["EmailOrSDT"] != null)
                ViewBag.EmailOrSDT = TempData["EmailOrSDT"].ToString();
            else
                ViewBag.EmailOrSDT = string.Empty;

            return View();
        }

        // POST: User/DoiMatKhau
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoiMatKhau(string EmailOrSDT, string MatKhauMoi, string XacNhanMatKhau)
        {
            if (string.IsNullOrWhiteSpace(EmailOrSDT))
            {
                ViewBag.Error = "Email hoặc SĐT không hợp lệ.";
                ViewBag.EmailOrSDT = EmailOrSDT;
                return View();
            }

            if (string.IsNullOrWhiteSpace(MatKhauMoi) || string.IsNullOrWhiteSpace(XacNhanMatKhau))
            {
                ViewBag.Error = "Vui lòng nhập mật khẩu mới và xác nhận mật khẩu.";
                ViewBag.EmailOrSDT = EmailOrSDT;
                return View();
            }

            var thongBao = db.Database.SqlQuery<string>(
                "EXEC DOIMATKHAU_EMAIL @EmailOrSDT, @MatKhauMoi, @XacNhanMatKhau;",
                new System.Data.SqlClient.SqlParameter("@EmailOrSDT", EmailOrSDT),
                new System.Data.SqlClient.SqlParameter("@MatKhauMoi", MatKhauMoi),
                new System.Data.SqlClient.SqlParameter("@XacNhanMatKhau", XacNhanMatKhau)
            ).FirstOrDefault();

            if (thongBao == "Đổi mật khẩu thành công!")
            {
                TempData["ThongBao"] = thongBao;
                return RedirectToAction("Login", "User");
            }

            ViewBag.Error = thongBao;
            ViewBag.EmailOrSDT = EmailOrSDT;
            return View();
        }
    }
}
