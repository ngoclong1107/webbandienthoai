
using DoAn1.Models;
using DoAnCoSo.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Data.Common;
using System.Data.Entity;

namespace DoAnCoSo.Controllers
{
    public class GioHangController : Controller
    {
        MyDataDataContext data = new MyDataDataContext();
        // GET: GioHang
        public List<GioHang> Laygiohang()
        {
            List<GioHang> listGiohang = Session["GioHang"] as List<GioHang>;
            if (listGiohang == null)
            {
                listGiohang = new List<GioHang>();
                Session["GioHang"] = listGiohang;
            }
            return listGiohang;
        }
        // GioHangController.cs
        public ActionResult ThemGioHang(int id, string strURL, string txtSoluong)
        {
            List<GioHang> listGioHang = Laygiohang();
            GioHang sanPham = listGioHang.Find(n => n.MaSP == id);

            if (sanPham == null)
            {
                int quantities = string.IsNullOrEmpty(txtSoluong) ? 1 : int.Parse(txtSoluong);
                sanPham = new GioHang(id, quantities);
                listGioHang.Add(sanPham);
            }
            else
            {
                int quantities = string.IsNullOrEmpty(txtSoluong) ? 1 : int.Parse(txtSoluong);
                sanPham.iSoluong += quantities;
            }

            Session["Giohang"] = listGioHang;
            return Redirect(strURL);
        }
        private int Tongsoluong()
        {
            int tsl = 0;
            List<GioHang> listgiohang = Session["GioHang"] as List<GioHang>;
            if (listgiohang != null)
            {
                tsl = listgiohang.Sum(n => n.iSoluong);
            }
            return tsl;
        }
        private int TongSoLuongSanPham()
        {
            int tsl = 0;
            List<GioHang> listgiohang = Session["GioHang"] as List<GioHang>;
            if (listgiohang != null)
            {
                tsl = listgiohang.Count;
            }
            return tsl;
        }
        private double TongTien()
        {
            double tt = 0;
            List<GioHang> listgiohang = Session["GioHang"] as List<GioHang>;
            if (listgiohang != null)
            {
                tt = listgiohang.Sum(n => n.dthanhtien);
            }
            return tt;
        }
        public ActionResult GioHang()
        {
            List<GioHang> listgiohang = Laygiohang();
            ViewBag.Tongsoluong = Tongsoluong();
            ViewBag.Tongtien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return View(listgiohang);
        }
        public ActionResult GioHangPartial()
        {
            ViewBag.Tongsoluong = Tongsoluong();
            ViewBag.Tongtien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return PartialView();
        }
        public ActionResult XoaGioHang(int id)
        {
            List<GioHang> listgiohang = Laygiohang();
            GioHang sanpham = listgiohang.SingleOrDefault(n => n.MaSP == id);
            if (sanpham != null)
            {
                listgiohang.RemoveAll(n => n.MaSP == id);
                return RedirectToAction("GioHang");
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult CapnhatGioHang(int id, FormCollection collection)
        {
            List<GioHang> listgiohang = Laygiohang();
            GioHang sanpham = listgiohang.SingleOrDefault(n => n.MaSP == id);
            if (sanpham != null)
            {
                sanpham.iSoluong = int.Parse(collection["txtSolg"].ToString());
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaTatCaGioHang()
        {
            List<GioHang> listgiohang = Laygiohang();
            listgiohang.Clear();
            return RedirectToAction("GioHang");
        }
        [HttpPost]
  
        public ActionResult DatHang(FormCollection collection, DonHang dh)
        {
            //Kiểm tra đăng đăng nhập
            //Kiểm tra giỏ hàng
            if (Session["GioHang"] == null)
            {
                RedirectToAction("Index", "Home");
            }
            var ngaydat = Convert.ToDateTime(collection["NgayDat"]);
            var tinhtrang = Convert.ToInt32(collection["TinhTrang"]);
            string thanhtoan = collection["MaTT"].ToString();
            int ptthanhtoan = Int32.Parse(thanhtoan);
            var diachinhanhang = collection["DiaChiNhanHang"];
            var tongtien = Convert.ToDecimal(collection["TongTien"]);

            tongtien = 0;
            List<GioHang> gh = Laygiohang();
            foreach (var item in gh)
            {
                decimal thanhtien = item.iSoluong * (decimal)item.dthanhtien;
                tongtien += thanhtien;
            }
            NguoiDung user = (NguoiDung)Session["member"];
            dh.MaNguoiDung = user.MaNguoiDung;
            dh.TongTien = tongtien;
            dh.NgayDat = DateTime.Now;
            dh.ThanhToan = ptthanhtoan;
            dh.DiaChiNhanHang = diachinhanhang;
            data.DonHangs.InsertOnSubmit(dh);
            data.SubmitChanges();

          
            foreach (var item in gh)
            {
                ChiTietDonHang ctDh = new ChiTietDonHang();
                ctDh.MaDon = dh.MaDon;
                ctDh.MaSP = item.MaSP;
                ctDh.SoLuong = item.iSoluong;
                ctDh.ThanhTien = (decimal)(item.iSoluong * item.giaban);
                ctDh.DonGia = (decimal)item.giaban;
                ctDh.PhuongThucThanhToan = 1;
                data.ChiTietDonHangs.InsertOnSubmit(ctDh);
                data.SubmitChanges();
                SanPham product = data.SanPhams.Where(p => p.MaSP == ctDh.MaSP).SingleOrDefault();
                if (product != null)
                {
                    product.SoLuong -= ctDh.SoLuong;
                    data.SubmitChanges();
                }
            }
           


            var ctdh = data.ChiTietDonHangs.Include(n => n.SanPham).Where(p=>p.MaDon  == dh.MaDon).ToList();
            var strSanPham = "";
            foreach (var sp in ctdh )
            {
                strSanPham += "<tr>";
                strSanPham += "<td>" + sp.SanPham.TenSP + "</td>";
                strSanPham += "<td>" + sp.SoLuong + "</td>";
                strSanPham += "<td>" + sp.DonGia + "</td>";
                strSanPham += "</tr>";

            }
            List<object> thanhToan = new List<object>
            {
                new { MaTT = 1, TenPT="Thanh toán tiền mặt" },
                new { MaTT = 2, TenPT="Thanh toán chuyển khoản" }
            };


            string phuongThucThanhToan = (string)thanhToan[0].GetType().GetProperty("TenPT").GetValue(thanhToan[0], null);

            string content = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/send2.html"));
            content = content.Replace("{{CustomerName}}", user.HoTen);
            content = content.Replace("{{Phone}}", user.DienThoai);
            content = content.Replace("{{Email}}", user.Email);
            content = content.Replace("{{Address}}", dh.DiaChiNhanHang);
            content = content.Replace("{{Total}}", dh.TongTien.ToString());
            content = content.Replace("{{MaDon}}", dh.MaDon.ToString());
            content = content.Replace("{{SanPham}}", strSanPham);
            content = content.Replace("{{NgayDat}}", dh.NgayDat.ToString());
            content = content.Replace("{{PhuongThucThanhToan}}", phuongThucThanhToan);

            var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
            new MailHelper().SendMail(user.Email, "Don hang toi tu ADP Shop", content);
            new MailHelper().SendMail(toEmail, "Don hang toi tu ADP Shop", content);
            if (dh.ThanhToan == 1)
            {
                return RedirectToAction("XacNhanDatHang", "GioHang");
            }
            else
            {
                return RedirectToAction("Payment", "GioHang");
            }
            //Thêm chi tiết đơn hàng
        }
      
        public ActionResult ThanhToanDonHang()
        {

            ViewBag.MaTT = new SelectList(new[]
                {
                    new { MaTT = 1, TenPT="Thanh toán tiền mặt" },
                    new { MaTT = 2, TenPT="Thanh toán chuyển khoản" },
                }, "MaTT", "TenPT", 1);
            ViewBag.MaNguoiDung = new SelectList(data.NguoiDungs, "MaNguoiDung", "HoTen");

            //Kiểm tra đăng đăng nhập
            if (Session["member"] == null || Session["member"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "User");
            }
            //Kiểm tra giỏ hàng
            if (Session["GioHang"] == null)
            {
                RedirectToAction("Index", "Home");
            }
            DonHang ddh = new DonHang();
            NguoiDung kh = (NguoiDung)Session["member"];
            List<GioHang> gh = Laygiohang();
            decimal tongtien = 0;
            foreach (var item in gh)
            {
                decimal thanhtien = item.iSoluong * (decimal)item.giaban;
                tongtien += thanhtien;
            }
            ddh.MaNguoiDung = kh.MaNguoiDung;
            ddh.NgayDat = DateTime.Now;
            ChiTietDonHang ctdh = new ChiTietDonHang();
            ViewBag.tongtien = tongtien;
            return View(ddh);

        }
        public ActionResult XacNhanDatHang()
        {
            return View();
        }
        public ActionResult Payment()
        {
            List<GioHang> gh = Laygiohang();
            decimal tongtien = 0;
            foreach (var item in gh)
            {
                decimal thanhtien = item.iSoluong * (decimal)item.giaban;
                tongtien += thanhtien;
            }
            tongtien = tongtien * 100;
            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", tongtien.ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 100000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang"); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

            return Redirect(paymentUrl);
        }

        public ActionResult PaymentConfirm()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }
                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }
            return View();
        }
    }
}