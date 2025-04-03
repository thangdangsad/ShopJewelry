using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebTrangSuc.Models; // DB First

namespace WebTrangSuc.Areas.Admin.Controllers
{
    public class HoaDonAdminController : Controller
    {
        private LiLyShopEntities5 db = new LiLyShopEntities5();

        // Hiển thị danh sách hóa đơn
        public ActionResult HoaDonAdmin()
        {
            ViewBag.ActiveMenu = "HoaDon";
            var hoaDons = db.HoaDons.Include(hd => hd.NguoiDung).ToList();

            return View(hoaDons);
        }

        // Trang chi tiết hóa đơn
        public ActionResult SuaHoaDon(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var hoaDon = db.HoaDons
                .Include(hd => hd.NguoiDung)
                .Include(hd => hd.ChiTietHoaDons.Select(ct => ct.SanPham))
                .Include(hd => hd.DiaChiNhanHang)
                .FirstOrDefault(hd => hd.HoaDonID == id);

            if (hoaDon == null)
                return HttpNotFound();

            // Nếu địa chỉ giao hàng là null, tạo địa chỉ "tạm" từ NguoiDung
            if (hoaDon.DiaChiNhanHang == null)
            {
                hoaDon.DiaChiNhanHang = new DiaChiNhanHang
                {
                    HoTen = hoaDon.NguoiDung.HoTen,
                    SoDienThoai = hoaDon.NguoiDung.SoDienThoai,
                    DiaChi = hoaDon.NguoiDung.DiaChi,
                    GhiChu = "Địa chỉ mặc định từ tài khoản"
                };
            }

            return View(hoaDon);
        }


        // Cập nhật trạng thái hóa đơn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhatHoaDon(int HoaDonID, string TrangThai)
        {
            var hoaDon = db.HoaDons.Find(HoaDonID);
            if (hoaDon == null) return HttpNotFound();

            hoaDon.TrangThai = TrangThai;
            db.SaveChanges();

            return RedirectToAction("HoaDonAdmin");
        }

        // Xóa hóa đơn (và chi tiết hóa đơn liên quan)
        [HttpPost]
        public ActionResult XoaHoaDon(int id)
        {
            var hoaDon = db.HoaDons.Find(id);
            if (hoaDon == null) return Json(new { success = false });

            db.ChiTietHoaDons.RemoveRange(db.ChiTietHoaDons.Where(ct => ct.HoaDonID == id));
            db.HoaDons.Remove(hoaDon);
            db.SaveChanges();

            return Json(new { success = true });
        }
    }
}
