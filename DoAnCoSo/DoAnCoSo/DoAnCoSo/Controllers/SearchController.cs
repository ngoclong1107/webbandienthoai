using DoAnCoSo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Web.UI;


namespace DoAnCoSo.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        MyDataDataContext data = new MyDataDataContext();
        [HttpGet]
        public ActionResult KQTimKiem(string sTuKhoa, int? page)
        {
            if (page == null) page = 1;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var listSP = data.SanPhams.AsQueryable();
            if (!string.IsNullOrEmpty(sTuKhoa))
            {
                listSP = listSP.Where(n => n.TenSP.Contains(sTuKhoa));
            }
            return View(listSP.OrderBy(n => n.TenSP).ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public ActionResult LayTuKhoaTimKiem(string sTuKhoa)
        {
            return RedirectToAction("KQTimKiem", new { @sTuKhoa = sTuKhoa });
        }
    }
}