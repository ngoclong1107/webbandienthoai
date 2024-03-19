using DoAnCoSo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace DoAnCoSo.Areas.Admin.Controllers
{
    public class DonHangController : Controller
    {
        // GET: Admin/Donhangs
        MyDataDataContext db = new MyDataDataContext();
        public ActionResult Index()
        {
            var donhangs = db.DonHangs.Include(d => d.NguoiDung);
            return View(donhangs.ToList());
        }
        // GET: Admin/Donhangs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var donhang = db.DonHangs.FirstOrDefault(d => d.MaDon == id);
            if (donhang == null)
            {
                return HttpNotFound();
            }
            return View(donhang);
        }
        public ActionResult Xacnhan(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var donhang = db.DonHangs.FirstOrDefault(d => d.MaDon == id);

            if (donhang == null)
            {
                return HttpNotFound();
            }

            donhang.TinhTrang = 1;
            db.SubmitChanges();

            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id)
        {
            var D_dh = db.DonHangs.First(m => m.MaDon == id);
            return View(D_dh);
        }
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var D_SP = db.DonHangs.Where(m => m.MaDon == id).First();
            db.DonHangs.DeleteOnSubmit(D_SP);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }
    }
}