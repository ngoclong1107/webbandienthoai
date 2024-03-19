using DoAnCoSo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;


namespace DoAnCoSo.Controllers
{
    public class DonHangController : Controller
    {
        // GET: DonHang
         MyDataDataContext db = new MyDataDataContext();

        // GET: Donhangs
        // Hiển thị danh sách đơn hàng
        public ActionResult Index()
        {
            //Kiểm tra đang đăng nhập
            if (Session["member"] == null || Session["member"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "User");
            }
            NguoiDung kh = (NguoiDung)Session["member"];
            int maND = kh.MaNguoiDung;
            var donhangs = db.DonHangs.Include(d => d.NguoiDung).Where(d => d.MaNguoiDung == maND);
            return View(donhangs.ToList());
        }

        // GET: Donhangs/Details/5
        //Hiển thị chi tiết đơn hàng
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var chitiet = db.ChiTietDonHangs.Include(d => d.SanPham).Where(d => d.MaDon == id).ToList();
            return View(chitiet);

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