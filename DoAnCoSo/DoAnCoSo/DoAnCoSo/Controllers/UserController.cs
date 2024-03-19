using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DoAnCoSo.Models;
using System.Web.Caching;
namespace DoAnCoSo.Controllers
{
    public class UserController : Controller
    {
        MyDataDataContext db = new MyDataDataContext();

        public string HashPassword(string password)
        {
            // Khởi tạo đối tượng SHA256
            using (var sha256 = SHA256.Create())
            {
                // Convert mật khẩu sang byte array
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Mã hóa mật khẩu
                byte[] hashedPasswordBytes = sha256.ComputeHash(passwordBytes);

                // Convert byte array sang chuỗi hexa
                string hashedPassword = BitConverter.ToString(hashedPasswordBytes).ToLower().Replace("-", "");

                return hashedPassword;
            }
        }
        public ActionResult Dangky()
        {
            return View();
        }
        // ĐĂNG KÝ PHƯƠNG THỨC POST
        [HttpPost]
        public ActionResult Dangky(NguoiDung nguoidung, string MatKhauXacNhan)
        {
            try
            {
              
                if (String.IsNullOrEmpty(MatKhauXacNhan))
                {
                    ViewData["NhapMKXN"] = "Phai nhap mat khau xac nhan";
                    return View("Dangky", nguoidung); // Pass the user object to the View
                }
                else
                {
                    string matkhau = nguoidung.MatKhau.ToString();
                    if (!matkhau.Equals(MatKhauXacNhan))
                    {
                        ViewData["MatKhauGiongNhau"] = "Mat khau va mat khau xac nhan phai giong nhau";
                        return View("Dangky", nguoidung); // Pass the user object to the View
                    }
                    else
                    {
                        nguoidung.IDQuyen = 2;
                        // Ma hoa mat khau truoc khi luu vao co so du lieu
                        nguoidung.MatKhau = HashPassword(nguoidung.MatKhau);

                        // Them nguoi dung moi
                        db.NguoiDungs.InsertOnSubmit(nguoidung);

                        // Luu lai vao co so du lieu
                        db.SubmitChanges();

                        // Neu du lieu dung thi tra ve trang dang nhap
                        ViewBag.RegOk = "Đăng kí thành công. Đăng nhập ngay";
                        ViewBag.isReg = true;
                        return RedirectToAction("Dangnhap");
                    }
                }
            }
            catch
            {
                return View();
            }
        }



        public ActionResult Dangnhap()
            {
                return View();

            }


        [HttpPost]
        public ActionResult Dangnhap(FormCollection userlog)
        {
            string userMail = userlog["email"].ToString();
            string password = userlog["matkhau"].ToString();

            // Mã hóa mật khẩu được nhập vào từ form đăng nhập
            string hashedPassword = HashPassword(password);

            var islogin = db.NguoiDungs.SingleOrDefault(x => x.Email == userMail && x.MatKhau == hashedPassword );

            if (islogin != null)
            {
                if (islogin.IDQuyen ==1)
                {
                    
                    Session["admin"] = islogin;
                    return RedirectToAction("Index", "Admin/Home");
                    
                }
                else
                {
                   
                    Session["member"] = islogin;
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.Fail = "Tài khoản hoặc mật khẩu không chính xác.";
                return View("Dangnhap");
            }
        }
        public ActionResult DangXuat()
        {
            Session["member"] = null;
            Session["admin"] = null;
            return RedirectToAction("Index", "Home");

        }

        public ActionResult Profile(int id)
        {
            NguoiDung nguoiDung = db.NguoiDungs.FirstOrDefault(m => m.MaNguoiDung == id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            return View(nguoiDung);
        }
        public ActionResult Edit(int id)
        {
            var E_Hang = db.NguoiDungs.First(m => m.MaNguoiDung == id);
            return View(E_Hang);
        }
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var E_ND = db.NguoiDungs.First(m => m.MaNguoiDung == id);
            var E_HoTen = collection["hoten"];
            var E_email = collection["email"];
            var E_dienthoai = collection["dienthoai"];
            var E_DiaChi = collection["diachi"];
            var E_anhdaidien = collection["anhdaidien"];
           
            E_ND.MaNguoiDung = id;
            if (string.IsNullOrEmpty(E_HoTen))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                E_ND.HoTen = E_HoTen;
                E_ND.Email = E_email;
                E_ND.DienThoai= E_dienthoai;
                E_ND.DiaChi = E_DiaChi;
                E_ND.AnhDaiDien = E_anhdaidien;
                UpdateModel(E_ND);
                db.SubmitChanges();
                return RedirectToAction("Profile" ,new { id = E_ND.MaNguoiDung });
            }
            return this.Edit(id);
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
        public ActionResult DoiMatKhau()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DoiMatKhau(string matkhau, string MatKhauXacNhan, string matKhauMoi)
        {
            NguoiDung user = (NguoiDung)Session["member"];
            var nguoiDung = db.NguoiDungs.FirstOrDefault(p => p.MaNguoiDung == user.MaNguoiDung);
            var matkhaucu = HashPassword(matkhau);

            // Kiểm tra mật khẩu cũ
            if (matkhaucu != nguoiDung.MatKhau)
            {
                ViewBag.Fail = "Mật khẩu cũ không đúng";
                return View("DoiMatKhau", user);
            }

            // Kiểm tra mật khẩu mới
            if (string.IsNullOrEmpty(matKhauMoi) || matKhauMoi.Length < 6)
            {
                ViewBag.Fail = "Mật khẩu mới phải có ít nhất 6 kí tự";
                return View("DoiMatKhau", user);
            }

            if (matKhauMoi != MatKhauXacNhan)
            {
                ViewBag.Fail = "Mật khẩu và mật khẩu xác nhận phải giống nhau";
                return View("DoiMatKhau", user);
            }
            else
            {
                // Lưu mật khẩu mới vào cơ sở dữ liệu
                user.MatKhau = HashPassword(matKhauMoi);
                UpdateModel(user);
                db.SubmitChanges();
                Session.Remove("member");
                return RedirectToAction("Dangnhap");
            }
            

        }
    }

}