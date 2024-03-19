using DoAnCoSo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCoSo.Areas.Admin.Controllers
{
    public class PhanQuyenController : Controller
    {
        // GET: Admin/PhanQuyen
        MyDataDataContext data = new MyDataDataContext();
        public ActionResult Index()
        {
            var all_pq = from ss in data.PhanQuyens select ss;
            return View(all_pq);
        }
        public ActionResult Detail(int id)
        {
            var D_SP = data.PhanQuyens.Where(m => m.IDQuyen == id).First();
            return View(D_SP);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection collection, PhanQuyen s)
        {
            var E_TenQuyen = collection["tenquyen"];
         
            if (string.IsNullOrEmpty(E_TenQuyen))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                s.TenQuyen = E_TenQuyen.ToString();
                data.PhanQuyens.InsertOnSubmit(s);
                data.SubmitChanges();
                return RedirectToAction("Index");
            }
            return this.Create();
        }
        public ActionResult Edit(int id)
        {
            var E_Hang = data.PhanQuyens.First(m => m.IDQuyen == id);
            return View(E_Hang);
        }
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var E_Quyen = data.PhanQuyens.First(m => m.IDQuyen == id);
            var E_TenQuyen = collection["tenquyen"];

            E_Quyen.IDQuyen = id;
            if (string.IsNullOrEmpty(E_TenQuyen))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                E_Quyen.TenQuyen = E_TenQuyen;
                UpdateModel(E_Quyen);
                data.SubmitChanges();
                return RedirectToAction("Index");
            }
            return this.Edit(id);
        }
        public ActionResult Delete(int id)
        {
            var D_sach = data.PhanQuyens.First(m => m.IDQuyen == id);
            return View(D_sach);
        }
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var D_HDH = data.PhanQuyens.Where(m => m.IDQuyen == id).First();
            data.PhanQuyens.DeleteOnSubmit(D_HDH);
            data.SubmitChanges();
            return RedirectToAction("Index");
        }
    }
}