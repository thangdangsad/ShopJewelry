using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTrangSuc.Models;

namespace WebTrangSuc.Controllers
{
    public class TrangSucCuoiController : Controller
    {
        private LiLyShopEntities5 db = new LiLyShopEntities5(); // Kết nối CSDL

        public ActionResult TrangSucCuoi(string sortOrder, string searchString, int page = 1)
        {
            int pageSize = 8;

            // Lấy danh sách sản phẩm thuộc loại Trang sức cưới
            var query = db.SanPhams.Where(sp => sp.IDLoaiSp == 1);

            // Áp dụng tìm kiếm nếu có từ khóa
            if (!string.IsNullOrEmpty(searchString))
            {
                string keyword = searchString.Trim().ToLower();
                query = query.Where(sp =>
                    sp.TenSanPham.ToLower().Contains(keyword) ||
                    (sp.MoTa != null && sp.MoTa.ToLower().Contains(keyword))
                );
            }

            // Sắp xếp sản phẩm theo tiêu chí
            switch (sortOrder)
            {
                case "gia_tang":
                    query = query.OrderBy(sp => (sp.GiaGiam != null && sp.GiaGiam > 0) ? sp.GiaGiam : sp.Gia);
                    break;
                case "gia_giam":
                    query = query.OrderByDescending(sp => (sp.GiaGiam != null && sp.GiaGiam > 0) ? sp.GiaGiam : sp.Gia);
                    break;
                case "ten_az":
                    query = query.OrderBy(sp => sp.TenSanPham);
                    break;
                case "ten_za":
                    query = query.OrderByDescending(sp => sp.TenSanPham);
                    break;
                default:
                    query = query.OrderBy(sp => sp.SanPhamID);
                    break;
            }

            // Đếm tổng số sản phẩm sau khi lọc và sắp xếp
            int totalItems = query.Count();

            // Phân trang sau khi đã sắp xếp
            var sanPhamsPaged = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.Page = page;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SearchString = searchString;

            // Thông báo nếu không tìm thấy
            if (!string.IsNullOrEmpty(searchString) && totalItems == 0)
            {
                ViewBag.SearchMessage = $"Không tìm thấy sản phẩm nào chứa từ khóa '{searchString}'";
            }

            return View(sanPhamsPaged);
        }

    }
}
