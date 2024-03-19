using DoAnCoSo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCoSo.Areas.Admin.Controllers
{
    public class ThongKeController : Controller
    {
        MyDataDataContext db = new MyDataDataContext();

        // GET: Admin/Thongkes

        public ActionResult RevenueStatistics(DateTime? fromDate, DateTime? toDate)
        {
            // Nếu không có tham số được truyền vào, set mặc định là 7 ngày trước
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                fromDate = DateTime.Now.AddDays(-7);
                toDate = DateTime.Now;
            }
            else
            {
                // Nếu có tham số, kiểm tra xem toDate có nhỏ hơn fromDate không
                if (toDate < fromDate)
                {
                    // Nếu có, hoán đổi lại giá trị của hai biến
                    var temp = toDate;
                    toDate = fromDate;
                    fromDate = temp;
                }
            }
            var items = db.DonHangs.Where(dh => dh.TinhTrang == 1 &&
                                                dh.NgayDat >= fromDate &&
                                                dh.NgayDat <= toDate)
                                    .OrderByDescending(dh => dh.NgayDat)
                                    .ToList();

            var dailyRevenue = items.GroupBy(dh => dh.NgayDat.Value.Date)
                             .Select(group => new
                             {
                                 Date = group.Key,
                                 Revenue = group.Sum(dh => dh.TongTien),
                                 Cost = group.Sum(dh => dh.ChiTietDonHangs.Sum(ct => ct.SanPham.GiaTien * ct.SoLuong))
                             })
                             .OrderBy(entry => entry.Date)
                             .ToList();

            ViewBag.Dates = dailyRevenue.Select(entry => entry.Date.ToString("dd/MM/yyyy"));
            ViewBag.Revenues = dailyRevenue.Select(entry => entry.Revenue);
            ViewBag.Costs = dailyRevenue.Select(entry => entry.Cost);
            ViewBag.Profits = dailyRevenue.Select(entry => entry.Revenue - entry.Cost);

            return View(items);
        }

    }
}