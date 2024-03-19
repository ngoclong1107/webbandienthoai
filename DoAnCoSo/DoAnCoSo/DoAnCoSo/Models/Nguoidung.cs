using DoAnCoSo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DoAnCoSo.Models
{
    public class Nguoidung
    {
        MyDataDataContext data = new MyDataDataContext();
        public int madon { get; set; }
        [Display(Name = "Ngay Dat")]
        public DateTime ngaydat { get; set; }
        [Display(Name = "Tinh Trang")]
        public int tinhtrang { get; set; }
        [Display(Name = "Thanh Toan")]
        public int thanhtoan { get; set; }
        [Display(Name = "Dia chi nhan hang")]
        public string diachinhanhang { get; set; }
        [Display(Name = "Tong Tien")]
        public decimal tongtien { get; set; }
        public int MaNguoiDung { get; internal set; }

        public Nguoidung(int id)
        {
            madon = id;
            DonHang dh = data.DonHangs.Single(n => n.MaDon == madon);
            ngaydat = (DateTime)dh.NgayDat;
            tinhtrang = (int)dh.TinhTrang;
            thanhtoan = (int)dh.ThanhToan;
            diachinhanhang = dh.DiaChiNhanHang;
            tongtien = (decimal)dh.TongTien;
        }
    }
}