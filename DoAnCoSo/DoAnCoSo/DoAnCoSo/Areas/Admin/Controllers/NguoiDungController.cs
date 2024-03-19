using DoAnCoSo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCoSo.Areas.Admin.Controllers
{
    public class NguoiDungController : Controller
    {
        // GET: Admin/NguoiDung
        MyDataDataContext data = new MyDataDataContext();
        public ActionResult Index()
        {
            var all_hdh = from ss in data.NguoiDungs select ss;
            return View(all_hdh);
        }
        public ActionResult Detail(int id)
        {
            var D_SP = data.NguoiDungs.Where(m => m.MaNguoiDung == id).First();
            return View(D_SP);
        }
        public ActionResult Delete(int id)
        {
            var D_sach = data.NguoiDungs.First(m => m.MaNguoiDung == id);
            return View(D_sach);
        }
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var D_HDH = data.NguoiDungs.Where(m => m.MaNguoiDung == id).First();
            data.NguoiDungs.DeleteOnSubmit(D_HDH);
            data.SubmitChanges();
            return RedirectToAction("Index");
        }
    }
}