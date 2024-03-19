
using DoAnCoSo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DoAnCoSo.Models
{
    public class Chitietdonhang
    {
        MyDataDataContext data = new MyDataDataContext();
        public int MaDon { get; set; }
        public int MaSP { get; set; }
        [Display(Name = "Phuong Thuc Thanh Toan")]
        public int phuongthucthanhtoan { get; set; }
        [Display(Name = "Gia Ban")]
        public double giaban { get; set; }
        [Display(Name = "So Luong")]
        public int iSoluong { get; set; }
        [Display(Name = "Thanh Tien")]
        public double dthanhtien { get { return iSoluong * giaban; } }
        public Chitietdonhang(int id)
        {
            MaDon = id;
            SanPham sanpham = data.SanPhams.Single(n => n.MaSP == MaSP);
            MaSP = sanpham.MaSP;
            giaban = double.Parse(sanpham.GiaTien.ToString());
            iSoluong = 1;
        }
    }
}