using DoAnCoSo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace DoAnCoSo.Controllers
{
    public class QuenMatKhauController : Controller
    {
        // GET: QuenMatKhau
        MyDataDataContext data = new MyDataDataContext();
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
        public ActionResult XacNhanmail()
        {
            return View();
        }
        [HttpPost]
        public ActionResult XacNhanMail(FormCollection formCollection)
        {
            var email = formCollection["email"];
            var nguoiDung = data.NguoiDungs.FirstOrDefault(p => p.Email == email);

            if (nguoiDung != null)
            {
                // Tạo mã xác nhận ngẫu nhiên
                // Tạo mã xác nhận ngẫu nhiên
                Random rand = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                string maXacNhan = new string(Enumerable.Repeat(chars, 6).Select(s => s[rand.Next(s.Length)]).ToArray());




                // Gửi email chứa mã xác nhận đến địa chỉ email của người dùng
                MailHelper mailHelper = new MailHelper();
                string subject = "Mã xác nhận đặt lại mật khẩu";
                string content = $"Chào {nguoiDung.HoTen},<br/>Đây là mã xác nhận của bạn để đặt lại mật khẩu: <b>{maXacNhan}</b>.<br/>Mã này sẽ hết hạn trong vòng 10 phút.";
                mailHelper.SendMail(nguoiDung.Email, subject, content);

                // Lưu thông tin xác nhận vào cơ sở dữ liệu
                var xacNhan = new XacNhanMatKhau
                {
                    MaXacNhan = maXacNhan,
                    MaNguoiDung = nguoiDung.MaNguoiDung,
                    ThoiGianTao = DateTime.Now,
                    ThoiGianHetHan = DateTime.Now.AddMinutes(10)
                };
                data.XacNhanMatKhaus.InsertOnSubmit(xacNhan);
                data.SubmitChanges();

                // Chuyển hướng đến trang nhập mã xác nhận
                return RedirectToAction("NhapMaXacNhan", new { maXacNhan = maXacNhan });
            }

            ViewBag.Fail = "Email không có trong cơ sở dữ liệu.";
            return View();
        }
        public ActionResult NhapMaXacNhan()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NhapMaXacNhan(string maXacNhan)
        {
            var xacNhan = data.XacNhanMatKhaus.FirstOrDefault(x => x.MaXacNhan == maXacNhan && x.ThoiGianHetHan >= DateTime.Now);

            if (xacNhan == null)
            {
                ViewBag.Fail = "Mã xác thực không đúng hoặc đã hết hạn.";
                return View();
            }
            else
            {
                Session["MaXacNhan"] = xacNhan.MaXacNhan;

                return RedirectToAction("TaoMatKhauMoi");
            }
        }

        public ActionResult TaoMatKhauMoi()
        {
            return View();
        }
        [HttpPost]
        public ActionResult TaoMatKhauMoi(string matKhauMoi, string MatKhauXacNhan)
        {

            var maXacNhan = Session["MaXacNhan"] as string;

            if (maXacNhan == null)
            {
                ViewBag.Fail = "Không tìm thấy mã xác nhận, có thể do phiên đăng nhập đã hết hạn.";
                return View();
            }

            var xacNhan = data.XacNhanMatKhaus.FirstOrDefault(x => x.MaXacNhan == maXacNhan);

            if (xacNhan == null || xacNhan.ThoiGianHetHan < DateTime.Now)
            {
                ViewBag.Fail = "Mã xác nhận không hợp lệ hoặc đã hết hạn.";
                return View();
            }

            var nguoiDung = data.NguoiDungs.FirstOrDefault(p => p.MaNguoiDung == xacNhan.MaNguoiDung);

            if (nguoiDung == null)
            {
                ViewBag.Fail = "Không tìm thấy người dùng tương ứng với mã xác nhận.";
                return View();
            }

            if (String.IsNullOrEmpty(MatKhauXacNhan))
            {
                ViewBag.Fail = "Phải nhập mật khẩu xác nhận";
                return View("TaoMatKhauMoi", nguoiDung); // Pass the user object to the View
            }
            else
            {
               
                if (matKhauMoi != MatKhauXacNhan)
                {
                    ViewBag.Fail = "Mật khẩu và mật khẩu xác nhận phải giống nhau"; 
                    return View("TaoMatKhauMoi", nguoiDung); // Pass the user object to the View
                }
               
                
                nguoiDung.MatKhau = HashPassword(matKhauMoi);
                UpdateModel(nguoiDung);
                data.SubmitChanges();
                


                // Xóa mã xác nhận khỏi cơ sở dữ liệu
                data.XacNhanMatKhaus.DeleteOnSubmit(xacNhan);
                data.SubmitChanges();

                // Xóa Session
                Session.Remove("MaXacNhan");

                return RedirectToAction("Dangnhap", "User");
            }


        }
    }
}