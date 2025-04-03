using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using WebTrangSuc.Models;

namespace WebTrangSuc.Controllers
{
    public class ThanhToanController : Controller
    {
        private LiLyShopEntities5 db = new LiLyShopEntities5();

        // GET: ThanhToan
        public ActionResult ThanhToan()
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

            if (!gioHangItems.Any())
            {
                return RedirectToAction("GioHang", "GioHang");
            }

            var nguoiDung = db.NguoiDungs.Find(nguoiDungID);
            ViewBag.NguoiDung = nguoiDung;

            var diaChiList = db.DiaChiNhanHangs
                               .Where(d => d.NguoiDungID == nguoiDungID)
                               .ToList();
            ViewBag.DiaChiList = diaChiList;

            return View(gioHangItems);
        }
        // POST: Xác nhận thanh toán
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XacNhanThanhToan(string HoTen, string SoDienThoai, string DiaChi, string GhiChu, int? DiaChiDropdown)
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

            if (!gioHangItems.Any())
            {
                return RedirectToAction("GioHang", "GioHang");
            }

            int diaChiID = 0;

            // Kiểm tra địa chỉ:
            if (DiaChiDropdown != null && DiaChiDropdown != 0)
            {
                diaChiID = DiaChiDropdown.Value;
            }
            else if (!string.IsNullOrEmpty(DiaChi)) // Thêm địa chỉ mới
            {
                var diaChiMoi = new DiaChiNhanHang
                {
                    NguoiDungID = nguoiDungID,
                    HoTen = HoTen,
                    SoDienThoai = SoDienThoai,
                    DiaChi = DiaChi,
                    GhiChu = GhiChu,
                    MacDinh = false
                };
                db.DiaChiNhanHangs.Add(diaChiMoi);
                db.SaveChanges();
                diaChiID = diaChiMoi.DiaChiID;
            }
            else
            {
                // Không chọn địa chỉ cũng không nhập mới -> hiển thị lại trang + báo lỗi
                var nguoiDung = db.NguoiDungs.Find(nguoiDungID);
                ViewBag.NguoiDung = nguoiDung;
                var diaChiList = db.DiaChiNhanHangs
                                   .Where(d => d.NguoiDungID == nguoiDungID)
                                   .ToList();
                ViewBag.DiaChiList = diaChiList;

                ViewBag.ErrorMessage = "Vui lòng chọn địa chỉ nhận hàng hoặc nhập địa chỉ mới.";
                return View("ThanhToan", gioHangItems);
            }

            // Tạo hóa đơn
            HoaDon hoaDon = new HoaDon
            {
                NguoiDungID = nguoiDungID,
                DiaChiID = diaChiID,
                NgayDat = DateTime.Now,
                TrangThai = "Chờ xử lý",
                TongTien = gioHangItems.Sum(g => (g.SoLuong ?? 0) * (g.SanPham.GiaGiam ?? g.SanPham.Gia))
            };
            db.HoaDons.Add(hoaDon);
            db.SaveChanges();

            // Chi tiết hóa đơn + cập nhật tồn kho
            foreach (var item in gioHangItems)
            {
                // Tạo chi tiết hóa đơn
                ChiTietHoaDon chiTiet = new ChiTietHoaDon
                {
                    HoaDonID = hoaDon.HoaDonID,
                    SanPhamID = item.SanPhamID,
                    SoLuong = item.SoLuong ?? 1,
                    DonGia = item.SanPham.GiaGiam ?? item.SanPham.Gia
                };
                db.ChiTietHoaDons.Add(chiTiet);

                // Cập nhật số lượng tồn kho
                var sanPham = db.SanPhams.Find(item.SanPhamID);
                if (sanPham != null)
                {
                    sanPham.SoLuong -= (item.SoLuong ?? 1);

                    // Nếu muốn tránh trường hợp âm số lượng thì thêm kiểm tra:
                    if (sanPham.SoLuong < 0)
                    {
                        sanPham.SoLuong = 0;
                    }
                    db.Entry(sanPham).State = EntityState.Modified;
                }
            }

            // Xóa giỏ hàng
            db.GioHangs.RemoveRange(gioHangItems);
            db.SaveChanges();

            return RedirectToAction("ThanhToanThanhCong");
        }

        // GET: Hoàn thành
        public ActionResult ThanhToanThanhCong()
        {
            return View();
        }
    }
}
