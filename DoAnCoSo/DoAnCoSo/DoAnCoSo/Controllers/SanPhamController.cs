using DoAnCoSo.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
namespace DoAnCoSo.Controllers
{
    public class SanPhamController : Controller
    {
       MyDataDataContext db  = new MyDataDataContext();
        public ActionResult PhuKien(int? page)
        { // 2. Nếu page = null thì đặt lại là 1.
            if (page == null) page = 1;
            int pageSize = 8;

            // 4.1 Toán tử ?? trong C# mô tả nếu page khác null thì lấy giá trị page, còn
            // nếu page = null thì lấy giá trị 1 cho biến pageNumber.
            int pageNumber = (page ?? 1);


            var dt = db.SanPhams.Where(n => n.MaLoai != 1).ToList();
            return PartialView(dt.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult DienThoai(int?page)
        { // 2. Nếu page = null thì đặt lại là 1.
            if (page == null) page = 1;
            int pageSize = 8;

            // 4.1 Toán tử ?? trong C# mô tả nếu page khác null thì lấy giá trị page, còn
            // nếu page = null thì lấy giá trị 1 cho biến pageNumber.
            int pageNumber = (page ?? 1);


            var dt = db.SanPhams.Where(n => n.MaLoai == 1).ToList();
            return PartialView(dt.ToPagedList(pageNumber,pageSize));
        }
        // GET: Sanpham
        public ActionResult DienThoaiIphone(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 8;

            // 4.1 Toán tử ?? trong C# mô tả nếu page khác null thì lấy giá trị page, còn
            // nếu page = null thì lấy giá trị 1 cho biến pageNumber.
            int pageNumber = (page ?? 1);
            var ip = db.SanPhams.Where(n => n.MaHang == 1).Take(4).ToList();
            return PartialView(ip.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult DienThoaiSamsung(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 8;

            // 4.1 Toán tử ?? trong C# mô tả nếu page khác null thì lấy giá trị page, còn
            // nếu page = null thì lấy giá trị 1 cho biến pageNumber.
            int pageNumber = (page ?? 1);
            var ss = db.SanPhams.Where(n => n.MaHang == 2).Take(4).ToList();
            return PartialView(ss.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult DienThoaiXiaomi(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 8;

            // 4.1 Toán tử ?? trong C# mô tả nếu page khác null thì lấy giá trị page, còn
            // nếu page = null thì lấy giá trị 1 cho biến pageNumber.
            int pageNumber = (page ?? 1);
            var mi = db.SanPhams.Where(n => n.MaHang == 3).Take(4).ToList();
            return PartialView(mi.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult DienThoaiOppo(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 8;

            // 4.1 Toán tử ?? trong C# mô tả nếu page khác null thì lấy giá trị page, còn
            // nếu page = null thì lấy giá trị 1 cho biến pageNumber.
            int pageNumber = (page ?? 1);
            var op = db.SanPhams.Where(n => n.MaHang == 4).Take(4).ToList();
            return PartialView(op.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Xemchitiet(int ?MaSP)
        {
            var chitiet = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            Session["sanpham"] = chitiet;
            if (chitiet == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(chitiet);
        }
        public ActionResult SubmitReview()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SubmitReview(FormCollection form)
        {
            NguoiDung user = (NguoiDung)Session["member"];
            var dg = new DanhGia();
            dg.NDDanhGia = form["message"];
            dg.Rating = Convert.ToInt32(form["rate"]);
            dg.MaNguoiDung = user.MaNguoiDung;
            SanPham sanpham = (SanPham)Session["sanpham"];
            dg.MaSP = sanpham.MaSP;
            dg.CommentDate = DateTime.Now;
            db.DanhGias.InsertOnSubmit(dg);
            db.SubmitChanges();


            // Truy vấn lại danh sách đánh giá của sản phẩm
            var danhgia = from ss in db.DanhGias.Where(p => p.MaSP == sanpham.MaSP) select ss;
            Session["danhgia"] = danhgia;

            // Điều hướng đến trang đánh giá sản phẩm và truyền danh sách đánh giá qua view
            return View("~/Views/SanPham/_DanhGiaPartial.cshtml", danhgia);
        }
    }
}