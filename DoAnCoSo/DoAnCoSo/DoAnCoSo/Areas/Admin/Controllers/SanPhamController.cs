using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using DoAnCoSo.Models;
using PagedList;

namespace DoAnCoSo.Areas.Admin.Controllers
{
    public class SanPhamController : Controller
    {
        // GET: Admin/SanPham
        MyDataDataContext data = new MyDataDataContext();
        public ActionResult Index(int? page)
        {  // 1. Tham số int? dùng để thể hiện null và kiểu int( số nguyên)
            // page có thể có giá trị là null ( rỗng) và kiểu int.

            // 2. Nếu page = null thì đặt lại là 1.
            if (page == null) page = 1;

            // 3. Tạo truy vấn sql, lưu ý phải sắp xếp theo trường nào đó, ví dụ OrderBy
            // theo Masp mới có thể phân trang.
            var sp = data.SanPhams.OrderBy(x => x.MaSP);

            // 4. Tạo kích thước trang (pageSize) hay là số sản phẩm hiển thị trên 1 trang
            int pageSize = 5;

            // 4.1 Toán tử ?? trong C# mô tả nếu page khác null thì lấy giá trị page, còn
            // nếu page = null thì lấy giá trị 1 cho biến pageNumber.
            int pageNumber = (page ?? 1);

            // 5. Trả về các sản phẩm được phân trang theo kích thước và số trang.
            return View(sp.ToPagedList(pageNumber, pageSize));

        }
        public ActionResult Detail(int id)
        {
            var D_SP = data.SanPhams.Where(m => m.MaSP == id).First();
            return View(D_SP);
        }
        public ActionResult Create()
        {
            ViewBag.MaHang = new SelectList(data.HangSanXuats, "MaHang", "TenHang");
            ViewBag.MaHDH = new SelectList(data.HeDieuHanhs, "MaHDH", "TenHDH");
            ViewBag.MaLoai = new SelectList(data.LoaiSanPhams, "MaLoai", "TenLoai");
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection collection, SanPham s)
        {
            var E_TenSP = collection["tensp"];
            var E_AnhBia = collection["anhbia"];
            var E_GiaTien = Convert.ToDecimal(collection["giatien"]);
            var E_SoLuong = Convert.ToInt32(collection["soluong"]);
            var E_MoTa = collection["mota"];
            var E_ChiTiet = collection["chitiet"];
            var E_MaLoai = Convert.ToInt32(collection["maloai"]);
            var E_MaHang = Convert.ToInt32(collection["mahang"]);
            var E_MaHDH = Convert.ToInt32(collection["mahdh"]);
            if (string.IsNullOrEmpty(E_TenSP))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                s.TenSP = E_TenSP.ToString();
                s.AnhBia = E_AnhBia.ToString();
                s.GiaTien = E_GiaTien;
                s.SoLuong = E_SoLuong;
                s.MoTa = E_MoTa.ToString();
                s.ChiTiet = E_ChiTiet.ToString();
                s.MaHang = E_MaHang;
                s.MaHDH = E_MaHDH;
                s.MaLoai = E_MaLoai;
                ViewBag.MaLoai = new SelectList(data.LoaiSanPhams, "MaLoai", "TenLoai", s.MaLoai);
                ViewBag.MaHang = new SelectList(data.HangSanXuats, "MaHang", "TenHang", s.MaHang);
                ViewBag.MaHDH = new SelectList(data.HeDieuHanhs, "MaHDH", "TenHDH", s.MaHDH);
                data.SanPhams.InsertOnSubmit(s);
                data.SubmitChanges();
                return RedirectToAction("Index");
            }
            return this.Create();
        }
        public ActionResult Edit(int id)
        {
            var E_SP = data.SanPhams.First(m => m.MaSP == id);
            ViewBag.MaHang = new SelectList(data.HangSanXuats, "MaHang", "TenHang");
            ViewBag.MaHDH = new SelectList(data.HeDieuHanhs, "MaHDH", "TenHDH");
            ViewBag.MaLoai = new SelectList(data.LoaiSanPhams, "MaLoai", "TenLoai");
            return View(E_SP);
        }
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var E_SP = data.SanPhams.First(m => m.MaSP == id);
            var E_TenSP = collection["tensp"];
            var E_AnhBia = collection["anhbia"];
            var E_GiaTien = Convert.ToDecimal(collection["giaban"]);
            var E_SoLuong = Convert.ToInt32(collection["soluongton"]);
            var E_MoTa = collection["mota"];
            var E_ChiTiet = collection["chitiet"];
            var E_MaLoai = Convert.ToInt32(collection["maloai"]);
            var E_MaHang = Convert.ToInt32(collection["mahang"]);
            var E_MaHDH = Convert.ToInt32(collection["mahdh"]);
            E_SP.MaSP = id;
            if (string.IsNullOrEmpty(E_TenSP))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                E_SP.TenSP = E_TenSP;
                E_SP.AnhBia = E_AnhBia;
                E_SP.GiaTien = E_GiaTien;
                E_SP.SoLuong = E_SoLuong;
                E_SP.MoTa = E_MoTa;
                E_SP.ChiTiet = E_ChiTiet;
                E_SP.MaHang = E_MaHang;
                E_SP.MaHDH = E_MaHDH;
                ViewBag.MaHang = new SelectList(data.HangSanXuats, "MaHang", "TenHang");
                ViewBag.MaHang = new SelectList(data.HangSanXuats, "MaHang", "TenHang", E_SP.MaHang);
                ViewBag.MaHDH = new SelectList(data.HeDieuHanhs, "MaHDH", "TenHDH", E_SP.MaHDH);
                UpdateModel(E_SP);
                data.SubmitChanges();
                return RedirectToAction("Index");
            }
            return this.Edit(id);
        }
        //-----------------------------------------
        public ActionResult Delete(int id)
        {
            var D_sach = data.SanPhams.First(m => m.MaSP == id);
            return View(D_sach);
        }
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var D_SP = data.SanPhams.Where(m => m.MaSP == id).First();
            data.SanPhams.DeleteOnSubmit(D_SP);
            data.SubmitChanges();
            return RedirectToAction("Index");
        }
        public string ProcessUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }
            file.SaveAs(Server.MapPath("~/Content/images/" + file.FileName));
            return "/Content/images/" + file.FileName;
        }
    }
}