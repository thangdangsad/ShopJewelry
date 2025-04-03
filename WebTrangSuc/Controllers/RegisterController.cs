using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using WebTrangSuc.Models;

namespace WebTrangSuc.Controllers
{
    public class RegisterController : Controller
    {
        private LiLyShopEntities5 db = new LiLyShopEntities5();

        // GET: Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string TenDangNhap, string Email, string MatKhau, string HoTen, string SoDienThoai, string DiaChi)
        {
            // Kiểm tra rỗng
            if (string.IsNullOrEmpty(TenDangNhap) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(MatKhau))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin bắt buộc!";
                return View();
            }

            // Kiểm tra định dạng Email
            if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ViewBag.Error = "Email không hợp lệ!";
                return View();
            }

            // Kiểm tra Username đã tồn tại
            if (db.NguoiDungs.Any(u => u.TenDangNhap == TenDangNhap))
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                return View();
            }

            // Kiểm tra Email đã tồn tại
            if (db.NguoiDungs.Any(u => u.Email == Email))
            {
                ViewBag.Error = "Email đã được sử dụng!";
                return View();
            }
            // Xử lý số điện thoại
            if (string.IsNullOrWhiteSpace(SoDienThoai))
            {
                ViewBag.Error = "Vui lòng nhập số điện thoại!";
                return View();
            }

            // Xóa dấu cách, ký tự thừa
            SoDienThoai = SoDienThoai.Trim().Replace(" ", "").Replace("\t", "");

            // Kiểm tra mã quốc gia
            if (SoDienThoai.StartsWith("+84"))
            {
                SoDienThoai = "0" + SoDienThoai.Substring(3);
            }

            // Kiểm tra định dạng: bắt đầu từ 03, 05, 07, 08, 09 và đủ 10 số
            if (!Regex.IsMatch(SoDienThoai, @"^(03|05|07|08|09)\d{8}$"))
            {
                ViewBag.Error = "Số điện thoại không hợp lệ! Phải bắt đầu bằng 03, 05, 07, 08, 09 và đủ 10 số.";
                return View();
            }


            // Lưu DB
            NguoiDung user = new NguoiDung()
            {
                TenDangNhap = TenDangNhap,
                Email = Email,
                MatKhau = MatKhau,
                HoTen = HoTen,
                SoDienThoai = SoDienThoai,
                DiaChi = DiaChi,
                VaiTro = "User",
                NgayDki = DateTime.Now
            };

            try
            {
                db.NguoiDungs.Add(user);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login", "Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi xảy ra: " + ex.Message;
                return View();
            }
        }
    }
}
