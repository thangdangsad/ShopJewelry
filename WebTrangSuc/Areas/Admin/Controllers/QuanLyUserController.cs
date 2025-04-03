using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebTrangSuc.Models;

namespace WebTrangSuc.Controllers
{
    public class QuanLyUserController : Controller
    {
        private LiLyShopEntities5 db = new LiLyShopEntities5();

        // Danh sách người dùng
        public ActionResult QuanLyUser(string search, string role)
        {
            var users = db.NguoiDungs.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => u.TenDangNhap.Contains(search) || u.HoTen.Contains(search) || u.Email.Contains(search));
            }

            if (!string.IsNullOrEmpty(role))
            {
                users = users.Where(u => u.VaiTro == role);
            }

            ViewBag.RoleFilter = new SelectList(new[] { "Admin", "User" });
            ViewBag.ActiveMenu = "QuanLyUser";
            return View(users.ToList());
        }

        // Hiển thị form thêm mới người dùng
        public ActionResult Create()
        {
            ViewBag.ActiveMenu = "QuanLyUser";
            return View();
        }

        // Xử lý thêm người dùng mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NguoiDung user)
        {
            // Kiểm tra dữ liệu nhập vào
            if (string.IsNullOrWhiteSpace(user.TenDangNhap))
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(user.MatKhau))
            {
                ModelState.AddModelError("MatKhau", "Mật khẩu không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                ModelState.AddModelError("Email", "Email không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(user.SoDienThoai))
            {
                ModelState.AddModelError("SoDienThoai", "Số điện thoại không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(user.VaiTro))
            {
                ModelState.AddModelError("VaiTro", "Vui lòng chọn vai trò.");
            }

            // Kiểm tra trùng lặp trong database
            bool isTenDangNhapExists = db.NguoiDungs.Any(u => u.TenDangNhap == user.TenDangNhap);
            bool isEmailExists = db.NguoiDungs.Any(u => u.Email == user.Email);

            if (isTenDangNhapExists)
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác.");
            }
            if (isEmailExists)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại. Vui lòng chọn email khác.");
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            // Lưu vào database
            db.NguoiDungs.Add(user);
            db.SaveChanges();
            return RedirectToAction("QuanLyUser");
        }



        // Hiển thị form chỉnh sửa
        public ActionResult Edit(int id)
        {
            ViewBag.ActiveMenu = "QuanLyUser";
            var user = db.NguoiDungs.Find(id);
            if (user == null) return HttpNotFound();
            return View(user);
        }

        // Xử lý cập nhật thông tin người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NguoiDung user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("QuanLyUser");
            }
            return View(user);
        }

        // Xóa người dùng
        public ActionResult Delete(int id)
        {
            var user = db.NguoiDungs.Find(id);
            if (user == null) return HttpNotFound();

            db.NguoiDungs.Remove(user);
            db.SaveChanges();
            return RedirectToAction("QuanLyUser");
        }
    }
}
