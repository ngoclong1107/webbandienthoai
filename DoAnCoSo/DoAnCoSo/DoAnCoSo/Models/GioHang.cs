using DoAnCoSo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DoAnCoSo.Models
{
    public class GioHang
    {

        MyDataDataContext data = new MyDataDataContext();
        public int  MaSP { get; set; }
        [Display(Name = "Ten San Pham")]
        public string TenSP { get; set; }
        [Display(Name = "Anh Bia")]
        public string AnhBia { get; set; }
        [Display(Name = "Gia Ban")]
        public double giaban { get; set; }
        [Display(Name = "So Luong")]
        public int iSoluong { get; set; }
        [Display(Name = "Thanh Tien")]
        public double dthanhtien { get { return iSoluong * giaban; } }
        public GioHang(int id, int quantities)
        {
            MaSP = id;
            SanPham sanpham = data.SanPhams.Single(n => n.MaSP == MaSP);
            TenSP = sanpham.TenSP;
            AnhBia = sanpham.AnhBia;
            giaban = double.Parse(sanpham.GiaTien.ToString());
            iSoluong = quantities;
        }
    }
}