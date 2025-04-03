using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTrangSuc.Models;

namespace WebTrangSuc.Areas.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        private LiLyShopEntities5 db = new LiLyShopEntities5();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["VaiTro"] == null || Session["VaiTro"].ToString() != "Admin")
            {
                filterContext.Result = RedirectToAction("Login", "Login", new { area = "" });
            }
            base.OnActionExecuting(filterContext);
        }
        // GET: Admin/HomeAdmin
        public ActionResult HomeAdmin()
        {
            ViewBag.ActiveMenu = "TrangChu";
            // Đếm số lượng từ bảng
            ViewBag.TotalUsers = db.NguoiDungs.Count();
            ViewBag.TotalProducts = db.SanPhams.Count();
            ViewBag.TotalOrders = db.HoaDons.Count();
            ViewBag.Revenue = db.ChiTietHoaDons.Sum(hd => (decimal?)hd.SoLuong * hd.DonGia) ?? 0;

            return View();
        }
    }
}