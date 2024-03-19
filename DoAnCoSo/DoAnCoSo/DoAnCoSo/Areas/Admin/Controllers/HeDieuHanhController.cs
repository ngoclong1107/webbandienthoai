using DoAnCoSo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCoSo.Areas.Admin.Controllers
{
    public class HeDieuHanhController : Controller
    {
        // GET: Admin/HeDieuHanh
        MyDataDataContext data = new MyDataDataContext();
        public ActionResult Index()
        {
            var all_hdh = from ss in data.HeDieuHanhs select ss;
            return View(all_hdh);
        }
        public ActionResult Detail(int id)
        {
            var D_SP = data.HeDieuHanhs.Where(m => m.MaHDH == id).First();
            return View(D_SP);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection collection, HeDieuHanh s)
        {
            var E_TenHDH = collection["tenhdh"];
            {
                ViewData["Error"] = "Don't empty!";
            }
            if (string.IsNullOrEmpty(E_TenHDH))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                s.TenHDH = E_TenHDH.ToString();
                data.HeDieuHanhs.InsertOnSubmit(s);
                data.SubmitChanges();
                return RedirectToAction("Index");
            }
            return this.Create();
        }
        public ActionResult Edit(int id)
        {
            var E_Hang = data.HeDieuHanhs.First(m => m.MaHDH == id);
            return View(E_Hang);
        }
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var E_Hang = data.HeDieuHanhs.First(m => m.MaHDH == id);
            var E_TenHDH = collection["tenhdh"];

            E_Hang.MaHDH = id;
            if (string.IsNullOrEmpty(E_TenHDH))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                E_Hang.TenHDH = E_TenHDH;
                UpdateModel(E_Hang);
                data.SubmitChanges();
                return RedirectToAction("Index");
            }
            return this.Edit(id);
        }
        public ActionResult Delete(int id)
        {
            var D_sach = data.HeDieuHanhs.First(m => m.MaHDH == id);
            return View(D_sach);
        }
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var D_HDH = data.HeDieuHanhs.Where(m => m.MaHDH == id).First();
            data.HeDieuHanhs.DeleteOnSubmit(D_HDH);
            data.SubmitChanges();
            return RedirectToAction("Index");
        }
    }
}