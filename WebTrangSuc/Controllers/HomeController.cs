using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTrangSuc.Models; // Import Model của bạn

namespace WebTrangSuc.Controllers
{
    public class HomeController : Controller
    {
        private readonly LiLyShopEntities5 db = new LiLyShopEntities5(); // Khởi tạo database context

        public ActionResult Home()
        {
            // Lấy 30 sản phẩm đầu tiên cho Product Promo Section
            var promoProducts = db.SanPhams
                        .Where(sp => sp.GiamGia != null)
                        .Take(28)
                        .ToList();

            // Lấy ngẫu nhiên 20 sản phẩm cho Product New Section
            var randomProducts = db.SanPhams
                                   .OrderBy(x => Guid.NewGuid()) // Random order
                                   .Take(20) // Số lượng tùy chỉnh
                                   .ToList();

            // Truyền dữ liệu sang View qua ViewBag
            ViewBag.PromoProducts = promoProducts;
            ViewBag.RandomProducts = randomProducts;
    //         // Lấy tất cả sản phẩm Trang sức (IDLoaiSp = 1)
    //var trangSucProducts = db.SanPhams
    //                         .Where(sp => sp.IDLoaiSp == 1)
    //                         .ToList();

    //// Lấy tất cả sản phẩm Đồng hồ (IDLoaiSp = 2)
    //var dongHoProducts = db.SanPhams
    //                       .Where(sp => sp.IDLoaiSp == 2)
    //                       .ToList();

    //ViewBag.TrangSucProducts = trangSucProducts;
    //ViewBag.DongHoProducts = dongHoProducts;

            return View();
        }
        [HttpGet]
        public JsonResult GetProductDetail(int id)
        {
            var product = db.SanPhams.FirstOrDefault(p => p.SanPhamID == id);
            if (product != null)
            {
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        product.SanPhamID,
                        product.TenSanPham,
                        product.HinhAnh,
                        product.Gia,
                        product.GiaGiam,
                        product.GiamGia,
                        product.SoLuong
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

    }
}
