using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebTrangSuc.Models;

namespace WebTrangSuc.Controllers
{
    public class GioHangController : Controller
    {
        private readonly LiLyShopEntities5 db = new LiLyShopEntities5();

        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public ActionResult ThemVaoGio(int id, int soLuong)
        {
            if (Session["NguoiDungID"] == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thêm vào giỏ hàng!" });
            }

            int nguoiDungID = Convert.ToInt32(Session["NguoiDungID"]);
            var gioHangItem = db.GioHangs.FirstOrDefault(g => g.NguoiDungID == nguoiDungID && g.SanPhamID == id);

            if (gioHangItem != null)
            {
                gioHangItem.SoLuong += soLuong;
            }
            else
            {
                gioHangItem = new GioHang
                {
                    NguoiDungID = nguoiDungID,
                    SanPhamID = id,
                    SoLuong = soLuong
                };
                db.GioHangs.Add(gioHangItem);
            }

            db.SaveChanges();

            // Cập nhật tổng số lượng giỏ hàng vào Session (Fix lỗi int?)
            int tongSoLuong = db.GioHangs
                                .Where(g => g.NguoiDungID == nguoiDungID)
                                .Sum(g => (int?)g.SoLuong) // ép kiểu sang int?
                                .GetValueOrDefault();     // lấy giá trị int, nếu null thì 0
            Session["SoLuongGioHang"] = tongSoLuong;

            return Json(new { success = true, soLuongGioHang = tongSoLuong });
        }

        // Xem giỏ hàng
        public ActionResult GioHang()
        {
            if (Session["NguoiDungID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            int nguoiDungID = Convert.ToInt32(Session["NguoiDungID"]);
            var gioHangItems = db.GioHangs
                                 .Where(g => g.NguoiDungID == nguoiDungID)
                                 .Include(g => g.SanPham)
                                 .ToList();

            // Cập nhật tổng số lượng vào Session
            int tongSoLuong = gioHangItems
                                .Sum(g => g.SoLuong.GetValueOrDefault()); // SoLuong có thể là int?, cần GetValueOrDefault()
            Session["SoLuongGioHang"] = tongSoLuong;

            return View(gioHangItems);
        }

        // Cập nhật số lượng
        [HttpPost]
        public ActionResult CapNhatSoLuong(int id, int soLuong)
        {
            if (Session["NguoiDungID"] == null)
            {
                return Json(new { success = false });
            }

            int nguoiDungID = Convert.ToInt32(Session["NguoiDungID"]);
            var gioHangItem = db.GioHangs.FirstOrDefault(g => g.NguoiDungID == nguoiDungID && g.SanPhamID == id);

            if (gioHangItem != null)
            {
                if (soLuong > 0)
                {
                    gioHangItem.SoLuong = soLuong;
                }
                else
                {
                    db.GioHangs.Remove(gioHangItem);
                }
                db.SaveChanges();

                // Cập nhật tổng số lượng
                int tongSoLuong = db.GioHangs
                                    .Where(g => g.NguoiDungID == nguoiDungID)
                                    .Sum(g => (int?)g.SoLuong)
                                    .GetValueOrDefault();
                Session["SoLuongGioHang"] = tongSoLuong;

                return Json(new { success = true, soLuongGioHang = tongSoLuong });
            }

            return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng." });
        }

        // Xóa sản phẩm
        [HttpPost]
        public ActionResult XoaKhoiGio(int id)
        {
            if (Session["NguoiDungID"] == null)
            {
                return Json(new { success = false });
            }

            int nguoiDungID = Convert.ToInt32(Session["NguoiDungID"]);
            var gioHangItem = db.GioHangs.FirstOrDefault(g => g.NguoiDungID == nguoiDungID && g.SanPhamID == id);

            if (gioHangItem != null)
            {
                db.GioHangs.Remove(gioHangItem);
                db.SaveChanges();

                // Cập nhật tổng số lượng
                int tongSoLuong = db.GioHangs
                                    .Where(g => g.NguoiDungID == nguoiDungID)
                                    .Sum(g => (int?)g.SoLuong)
                                    .GetValueOrDefault();
                Session["SoLuongGioHang"] = tongSoLuong;

                return Json(new { success = true, soLuongGioHang = tongSoLuong });
            }

            return Json(new { success = false, message = "Sản phẩm không tồn tại." });
        }
    }
}
