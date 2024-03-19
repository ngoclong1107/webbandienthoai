using DoAnCoSo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCoSo.Controllers
{
    public class DanhMucController : Controller
    {
        MyDataDataContext db = new MyDataDataContext();
        // GET: Danhmuc
        public ActionResult DanhmucPartial()
        {
            var danhmuc = db.HangSanXuats.ToList();
            return View(danhmuc);
        }
    }
}