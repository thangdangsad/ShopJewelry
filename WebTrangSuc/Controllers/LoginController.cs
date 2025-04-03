using System.Linq;
using System.Web.Mvc;
using WebTrangSuc.Models;

namespace WebTrangSuc.Controllers
{
    public class LoginController : Controller
    {
        private LiLyShopEntities5 db = new LiLyShopEntities5();

        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string TenDangNhap, string MatKhau)
        {
            // Kiểm tra nhập thiếu
            if (string.IsNullOrEmpty(TenDangNhap) || string.IsNullOrEmpty(MatKhau))
            {
                ViewBag.Message = "Vui lòng nhập đầy đủ Tên đăng nhập và Mật khẩu!";
                return View();
            }

            // Kiểm tra người dùng
            var user = db.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == TenDangNhap && u.MatKhau == MatKhau);

            if (user != null)
            {
                // Lưu thông tin Session
                Session["NguoiDungID"] = user.NguoiDungID;
                Session["TenDangNhap"] = user.TenDangNhap;
                Session["HoTen"] = user.HoTen;
                Session["Email"] = user.Email;
                Session["SoDienThoai"] = user.SoDienThoai;
                Session["VaiTro"] = user.VaiTro;

                if (user.VaiTro == "Admin")
                {
                    return RedirectToAction("HomeAdmin", "HomeAdmin", new { area = "Admin" });
                }
                else
                {
                    return RedirectToAction("Home", "Home");
                }
            }

            ViewBag.Message = "Sai thông tin đăng nhập!";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Home", "Home");
        }

        public ActionResult Account()
        {
            if (Session["NguoiDungID"] == null)
            {
                return RedirectToAction("Login");
            }

            int userId = (int)Session["NguoiDungID"];
            var user = db.NguoiDungs.SingleOrDefault(x => x.NguoiDungID == userId);

            return View(user);
        }
    }
}
