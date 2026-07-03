using System;
using System.Linq;
using System.Web.Mvc;
using Website_Ban_Nuoc_Online.Models;
using System.Web.Security;

namespace QL_QuanCF.Controllers
{
    public class AdminController : Controller
    {
        private WebsiteBanNuocContext db = new WebsiteBanNuocContext();

        [HttpGet]
        public ActionResult Auth()
        {
            // Nếu có TempData["ThongBao"] hoặc TempData["Error"], view sẽ hiển thị
            return View(); // Views/Admin/Auth.cshtml
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // tên action hiện tại
            string action = filterContext.ActionDescriptor.ActionName;

            // Các action cho phép vào mà không cần đăng nhập admin
            string[] allowAnonymous =
            {
        "Auth", "Login", "Register",
        "QuenMatKhau", "DoiMatKhau"
    };

            // Nếu action nằm trong allowAnonymous thì cho qua
            if (allowAnonymous.Contains(action, StringComparer.OrdinalIgnoreCase))
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            // Còn lại: bắt buộc phải là admin
            if (Session["Admin"] == null)
            {
                filterContext.Result = RedirectToAction("Auth", "Admin");
                return;
            }

            base.OnActionExecuting(filterContext);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Username, string Password)
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                TempData["Error"] = "Vui lòng nhập username và mật khẩu.";
                return RedirectToAction("Auth");
            }

            var admin = db.Admins.FirstOrDefault(a => a.Username == Username && a.Password == Password);
            if (admin == null)
            {
                TempData["Error"] = "Username hoặc mật khẩu không đúng!";
                TempData["ShowLogin"] = true; // đảm bảo mở đúng tab Login
                return RedirectToAction("Auth");
            }

            // LƯU ADMIN VÀO SESSION
            Session["Admin"] = admin;
            Session["IsAdmin"] = true;   // nếu bạn còn dùng IsAdmin ở layout

            TempData["ThongBao"] = "Đăng nhập thành công!";
            return RedirectToAction("Index", "QuanLy");
        }


        // Register: không dùng ViewModel mới, lấy dữ liệu từ FormCollection để dễ map tên input
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(FormCollection f)
        {
            // Lấy các giá trị từ form
            string Username = (f["Username"] ?? "").Trim();
            string FullName = (f["FullName"] ?? "").Trim();
            string Password = (f["Password"] ?? "").Trim();
            string ConfirmPassword = (f["ConfirmPassword"] ?? "").Trim();
            string Email = (f["Email"] ?? "").Trim();
            string Phone = (f["Phone"] ?? "").Trim();
            string CCCD = (f["CCCD"] ?? "").Trim();
            string Date = (f["Date"] ?? "").Trim();
            string Gender = (f["Gender"] ?? "").Trim();
            string Address = (f["Address"] ?? "").Trim();

            // --- VALIDATE TỪNG TRƯỜNG ---
            if (string.IsNullOrWhiteSpace(Username))
                ModelState.AddModelError("Username", "Nhập thiếu username");

            if (string.IsNullOrWhiteSpace(FullName))
                ModelState.AddModelError("FullName", "Nhập thiếu họ tên");

            if (string.IsNullOrWhiteSpace(Password))
                ModelState.AddModelError("Password", "Nhập thiếu mật khẩu");
            else if (Password.Length < 6)
                ModelState.AddModelError("Password", "Mật khẩu phải >= 6 ký tự");

            if (string.IsNullOrWhiteSpace(ConfirmPassword))
                ModelState.AddModelError("ConfirmPassword", "Vui lòng xác nhận mật khẩu");

            if (!string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(ConfirmPassword) && Password != ConfirmPassword)
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp");

            if (string.IsNullOrWhiteSpace(Email))
                ModelState.AddModelError("Email", "Nhập thiếu email");

            if (string.IsNullOrWhiteSpace(Phone))
                ModelState.AddModelError("Phone", "Nhập thiếu số điện thoại");

            if (string.IsNullOrWhiteSpace(CCCD))
                ModelState.AddModelError("CCCD", "Nhập thiếu căn cước công dân");

            if (string.IsNullOrWhiteSpace(Date))
                ModelState.AddModelError("Date", "Nhập thiếu ngày sinh");

            if (string.IsNullOrWhiteSpace(Gender))
                ModelState.AddModelError("Gender", "Vui lòng chọn giới tính");

            if (string.IsNullOrWhiteSpace(Address))
                ModelState.AddModelError("Address", "Nhập thiếu địa chỉ");

            // Ví dụ thêm validate CCCD (nếu cần)
            if (!string.IsNullOrWhiteSpace(CCCD) && (CCCD.Length != 12 && CCCD.Length != 9))
                ModelState.AddModelError("CCCD", "CCCD không hợp lệ (9 hoặc 12 chữ số)");

            // Nếu có lỗi -> quay lại view Auth và bật tab Đăng ký
            if (!ModelState.IsValid)
            {
                ViewBag.ShowRegister = true; // để view tự bật tab Đăng ký khi render
                return View("Auth");
            }

            // Kiểm tra username tồn tại
            if (db.Admins.Any(a => a.Username.Equals(Username, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Username", "Username đã tồn tại");
                ViewBag.ShowRegister = true;
                return View("Auth");
            }

            // Lưu dữ liệu (chú ý: trong dự án thật cần hash mật khẩu)
            var admin = new Admin
            {
                Username = Username,
                FullName = FullName,
                Password = Password, // TODO: hash + salt trước khi lưu
                Email = Email,
                Phone = Phone,
                CCCD = CCCD,
                Gender = Gender,
                Address = Address,
                CreatedAt = DateTime.Now
            };

            db.Admins.Add(admin);
            db.SaveChanges();

            // Sau khi đăng ký thành công: hiển thị thông báo và chuyển sang tab Đăng nhập
            TempData["ThongBao"] = "Đăng ký thành công! Bạn có thể đăng nhập ngay.";
            TempData["ShowLogin"] = true; // để view tự bật tab Đăng nhập sau redirect
            return RedirectToAction("Auth");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Session["Admin"] = null;
            Session.Clear();
            Session.Abandon();

            try { FormsAuthentication.SignOut(); } catch { /* ignore */ }

            TempData["ThongBao"] = "Bạn đã đăng xuất.";
            return RedirectToAction("Auth");
        }

        // GET: Admin/QuenMatKhau
        [HttpGet]
        public ActionResult QuenMatKhau()
        {
            return View();
        }

        // POST: Admin/QuenMatKhau
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuenMatKhau(string Username, string EmailOrSDT, string CCCD)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(EmailOrSDT) || string.IsNullOrWhiteSpace(CCCD))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ: Username, Email hoặc SĐT và CCCD.";
                return View();
            }

            // Tìm admin theo Username và (Email hoặc Phone) và CCCD
            var admin = db.Admins.FirstOrDefault(a =>
                a.Username.Equals(Username, StringComparison.OrdinalIgnoreCase) &&
                (a.Email == EmailOrSDT || a.Phone == EmailOrSDT) &&
                a.CCCD == CCCD);

            if (admin == null)
            {
                ViewBag.Error = "Không tìm thấy quản trị viên nào khớp với thông tin đã cung cấp.";
                return View();
            }

            // Thông tin khớp -> chuyển sang form đổi mật khẩu
            TempData["AdminUserForReset"] = admin.Username;
            return RedirectToAction("DoiMatKhau");
        }

        // GET: Admin/DoiMatKhau
        [HttpGet]
        public ActionResult DoiMatKhau()
        {
            if (TempData["AdminUserForReset"] != null)
            {
                ViewBag.Username = TempData["AdminUserForReset"].ToString();
            }
            else if (Request.QueryString["Username"] != null)
            {
                ViewBag.Username = Request.QueryString["Username"];
            }
            else
            {
                ViewBag.Username = string.Empty;
            }

            return View();
        }

        // POST: Admin/DoiMatKhau
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoiMatKhau(string Username, string MatKhauMoi, string XacNhanMatKhau)
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                ViewBag.Error = "Tên đăng nhập không hợp lệ.";
                ViewBag.Username = Username;
                return View();
            }

            if (string.IsNullOrWhiteSpace(MatKhauMoi) || string.IsNullOrWhiteSpace(XacNhanMatKhau))
            {
                ViewBag.Error = "Vui lòng nhập mật khẩu mới và xác nhận mật khẩu.";
                ViewBag.Username = Username;
                return View();
            }

            if (MatKhauMoi != XacNhanMatKhau)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                ViewBag.Username = Username;
                return View();
            }

            var admin = db.Admins.FirstOrDefault(a => a.Username.Equals(Username, StringComparison.OrdinalIgnoreCase));
            if (admin == null)
            {
                ViewBag.Error = "Không tìm thấy tài khoản quản trị viên.";
                ViewBag.Username = Username;
                return View();
            }

            // Cập nhật mật khẩu (hiện đang dùng plaintext theo dự án)
            admin.Password = MatKhauMoi;
            db.Entry(admin).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            TempData["ThongBao"] = "Đổi mật khẩu thành công! Vui lòng đăng nhập bằng mật khẩu mới.";
            return RedirectToAction("Auth", "Admin");
        }
    }
}
