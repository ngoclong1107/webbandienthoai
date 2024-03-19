using DoAnCoSo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCoSo.Areas.Admin.Controllers
{
    public class HangSanXuatController : Controller
    {
        // GET: Admin/HangSanXuat
        MyDataDataContext data = new MyDataDataContext();
        public ActionResult Index()
        {
            var all_hang = from ss in data.HangSanXuats select ss;
            return View(all_hang);
        }
        public ActionResult Detail(int id)
        {
            var D_SP = data.HangSanXuats.Where(m => m.MaHang == id).First();
            return View(D_SP);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection collection, HangSanXuat s)
        {
            var E_TenHang = collection["TenHang"];
            {
                ViewData["Error"] = "Don't empty!";
            }
            if (string.IsNullOrEmpty(E_TenHang))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                s.TenHang = E_TenHang.ToString();
                data.HangSanXuats.InsertOnSubmit(s);
                data.SubmitChanges();
                return RedirectToAction("Index");
            }
            return this.Create();
        }
        public ActionResult Edit(int id)
        {
            var E_Hang = data.HangSanXuats.First(m => m.MaHang == id);
            return View(E_Hang);
        }
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var E_Hang = data.HangSanXuats.First(m => m.MaHang == id);
            var E_TenHang = collection["TenHang"];

            E_Hang.MaHang = id;
            if (string.IsNullOrEmpty(E_TenHang))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                E_Hang.TenHang = E_TenHang;
                UpdateModel(E_Hang);
                data.SubmitChanges();
                return RedirectToAction("Index");
            }
            return this.Edit(id);
        }
        public ActionResult Delete(int id)
        {
            var D_sach = data.HangSanXuats.First(m => m.MaHang == id);
            return View(D_sach);
        }
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var D_SP = data.HangSanXuats.Where(m => m.MaHang == id).First();
            data.HangSanXuats.DeleteOnSubmit(D_SP);
            data.SubmitChanges();
            return RedirectToAction("Index");
        }
    }
}