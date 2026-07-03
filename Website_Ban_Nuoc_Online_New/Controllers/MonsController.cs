using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Website_Ban_Nuoc_Online.Models;

namespace QL_QuanCF.Controllers
{
    public class MonsController : Controller
    {
        private WebsiteBanNuocContext db = new WebsiteBanNuocContext();

        // GET: Mons
        public ActionResult Index()
        {
            var mons = db.Mons.Include(m => m.Loai);
            return View(mons.ToList());
        }
        public ActionResult DSMenu()
        {
            List<Loai> dscd = db.Loais.Take(10).ToList();
            return PartialView(dscd);
        }
        public ActionResult HTSachTheoLoai(int? MaLoai)
        {
            List<Mon> sanphamtheoDM = db.Mons.Where(x => x.MaLoai == MaLoai).ToList();
            return View("Index", sanphamtheoDM);
        }

        // GET: Mons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mon mon = db.Mons.Find(id);
            if (mon == null)
            {
                return HttpNotFound();
            }
            return View(mon);
        }

        // GET: Mons/Create
        public ActionResult Create()
        {
            ViewBag.MaLoai = new SelectList(db.Loais, "MaLoai", "TenLoai");
            return View();
        }

        // POST: Mons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaMon,TenMon,GiaBan,MoTa,HinhAnh,MaLoai")] Mon mon)
        {
            if (ModelState.IsValid)
            {
                db.Mons.Add(mon);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaLoai = new SelectList(db.Loais, "MaLoai", "TenLoai", mon.MaLoai);
            return View(mon);
        }

        // GET: Mons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mon mon = db.Mons.Find(id);
            if (mon == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaLoai = new SelectList(db.Loais, "MaLoai", "TenLoai", mon.MaLoai);
            return View(mon);
        }

        // POST: Mons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaMon,TenMon,GiaBan,MoTa,HinhAnh,MaLoai")] Mon mon)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mon).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaLoai = new SelectList(db.Loais, "MaLoai", "TenLoai", mon.MaLoai);
            return View(mon);
        }

        // GET: Mons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mon mon = db.Mons.Find(id);
            if (mon == null)
            {
                return HttpNotFound();
            }
            return View(mon);
        }

        // POST: Mons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Mon mon = db.Mons.Find(id);
            db.Mons.Remove(mon);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
