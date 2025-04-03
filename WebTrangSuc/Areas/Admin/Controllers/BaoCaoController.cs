using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebTrangSuc.Models;
using Newtonsoft.Json;
using System.Data.Entity;

namespace WebTrangSuc.Areas.Admin.Controllers
{
   
    public class BaoCaoController : Controller
    {
        private readonly LiLyShopEntities5 db = new LiLyShopEntities5();

        public ActionResult BaoCao()
        {
            ViewBag.ActiveMenu = "BaoCao";
            return View();
        }

        [HttpGet]
        public JsonResult GetDoanhThu(string type)
        {
            DateTime startDate = DateTime.Now;
            switch (type)
            {
                case "week":
                    startDate = DateTime.Now.AddDays(-7);
                    break;
                case "month":
                    startDate = DateTime.Now.AddMonths(-1);
                    break;
                case "year":
                    startDate = DateTime.Now.AddYears(-1);
                    break;
            }

            var data = db.HoaDons
            .Where(h => h.NgayDat >= startDate)
            .GroupBy(h => System.Data.Entity.DbFunctions.TruncateTime(h.NgayDat))
            .ToList() // Lấy dữ liệu ra trước
            .Select(g => new
            {
                Date = g.Key?.ToString("dd/MM/yyyy"), // Sử dụng ?. để tránh lỗi null
                Revenue = g.Sum(h => h.TongTien)
            }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

      [HttpGet]
public JsonResult GetSanPhamBanChay()
{
    var data = db.ChiTietHoaDons
        .GroupBy(c => c.SanPham.TenSanPham)
        .Select(g => new
        {
            TenSanPham = g.Key,
            SoLuongBan = g.Sum(c => c.SoLuong)
        })
        .Where(g => g.SoLuongBan >= 8) // Lọc sản phẩm có số lượng bán >= 8
        .OrderByDescending(g => g.SoLuongBan) // Sắp xếp từ nhiều đến ít
        .Take(10)
        .ToList();

    return Json(data, JsonRequestBehavior.AllowGet);
}

[HttpGet]
public JsonResult GetSanPhamItMua()
{
    var data = db.ChiTietHoaDons
        .GroupBy(c => c.SanPham.TenSanPham)
        .Select(g => new
        {
            TenSanPham = g.Key,
            SoLuongBan = g.Sum(c => c.SoLuong)
        })
        .Where(g => g.SoLuongBan < 8) // Lọc sản phẩm có số lượng bán < 8
        .OrderBy(g => g.SoLuongBan) // Sắp xếp từ ít đến nhiều
        .Take(10)
        .ToList();

    return Json(data, JsonRequestBehavior.AllowGet);
}


        [HttpGet]
        public JsonResult GetSanPhamSapHetHang()
        {
            var data = db.SanPhams
                .Where(s => s.SoLuong < 8)
                .Select(s => new
                {
                    s.TenSanPham,
                    s.SoLuong
                }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public JsonResult GetBaoCaoDonHang(string type)
        {
            DateTime startDate = DateTime.Today;
            switch (type)
            {
                case "week":
                    startDate = DateTime.Today.AddDays(-7);
                    break;
                case "month":
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    break;
                case "year":
                    startDate = new DateTime(DateTime.Today.Year, 1, 1);
                    break;
            }

            var data = db.HoaDons
             .Where(h => h.NgayDat >= startDate)
             .GroupBy(h => DbFunctions.TruncateTime(h.NgayDat))
             .AsEnumerable()
             .Select(g => new
             {
                 Date = g.Key.HasValue ? g.Key.Value.ToString("dd/MM/yyyy") : "Không xác định",
                 OrderCount = g.Count()
             })
             .ToList();


            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTinhTrangDonHang(string type)
        {
            DateTime startDate = DateTime.Now;
            switch (type)
            {
                case "week":
                    startDate = DateTime.Now.AddDays(-7);
                    break;
                case "month":
                    startDate = DateTime.Now.AddMonths(-1);
                    break;
                case "year":
                    startDate = DateTime.Now.AddYears(-1);
                    break;
            }

            var data = db.HoaDons
         .Where(h => h.NgayDat >= startDate)
         .GroupBy(h => h.TrangThai)
         .Select(g => new
         {
             Status = g.Key ?? "Không xác định",  // Trạng thái "Không xác định" khi null
             Count = g.Count()
         })
         .OrderBy(g => g.Status) // Thêm dòng này để sắp xếp theo trạng thái, nếu cần
         .ToList();


            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBaoCaoNguoiDungMoi(string type)
        {
            DateTime startDate = DateTime.Now;
            switch (type)
            {
                case "week":
                    startDate = DateTime.Now.AddDays(-7);
                    break;
                case "month":
                    startDate = DateTime.Now.AddMonths(-1);
                    break;
                case "year":
                    startDate = DateTime.Now.AddYears(-1);
                    break;
            }

            var data = db.NguoiDungs
            .Where(u => u.NgayDki >= startDate)
            .GroupBy(u => DbFunctions.TruncateTime(u.NgayDki))
            .AsEnumerable()
            .Select(g => new
            {
                Date = g.Key.HasValue ? g.Key.Value.ToString("dd/MM/yyyy") : "Không xác định",
                UserCount = g.Count()
            })
            .ToList();


            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
