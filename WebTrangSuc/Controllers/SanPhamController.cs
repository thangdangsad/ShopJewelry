using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebTrangSuc.Models;

namespace WebTrangSuc.Controllers
{
    public class SanPhamController : Controller
    {
        private LiLyShopEntities5 db = new LiLyShopEntities5();
        public ActionResult ChiTiet(int id)
        {
            var sanPham = db.SanPhams.FirstOrDefault(sp => sp.SanPhamID == id);

            if (sanPham == null)
            {
                return HttpNotFound();
            }

            var sanPhamTuongTu = db.SanPhams
                .Where(sp => sp.SanPhamID != id)
                .OrderBy(r => Guid.NewGuid())
                .Take(4)
                .ToList();

            ViewBag.SanPhamTuongTu = sanPhamTuongTu;

            return View("ChiTietSanPham", sanPham); // View tên gì thì ghi đúng tên file cshtml
        }



    }
}
