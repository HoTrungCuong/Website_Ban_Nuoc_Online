using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_Ban_Nuoc_Online.Models;

namespace QL_QuanCF.Controllers
{
    public class HomeController : Controller
    {
        private WebsiteBanNuocContext db = new WebsiteBanNuocContext();
        public ActionResult Index()
        {
            var sanPhamTieuBieu = db.Mons
                                    .OrderByDescending(sp => sp.MaMon)
                                    .Take(4) // lấy 4 sản phẩm mới nhất
                                    .ToList();

            ViewBag.SanPhamTieuBieu = sanPhamTieuBieu;
            return View();
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult NapTien()
        {
            return View();
        }
    }
}