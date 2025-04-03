using System.Linq;
using System.Web.Mvc;
using WebTrangSuc.Models;

namespace WebTrangSuc.Controllers
{
    public class ThongTinTkController : Controller
    {
        private LiLyShopEntities5 db = new LiLyShopEntities5();

        // GET: ThongTinTk/TaiKhoan
        public ActionResult TaiKhoan()
        {
            // Kiểm tra đăng nhập
            if (Session["NguoiDungID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            int userId = (int)Session["NguoiDungID"];
            var user = db.NguoiDungs.SingleOrDefault(x => x.NguoiDungID == userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user); // Truyền model user sang View
        }
    }
}
