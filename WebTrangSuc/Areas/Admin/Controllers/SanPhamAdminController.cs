using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebTrangSuc.Models;

namespace WebTrangSuc.Controllers
{
    public class SanPhamAdminController : Controller
    {
        private LiLyShopEntities5 db = new LiLyShopEntities5();
        public ActionResult Info(string tenSanPham, decimal? giaTu, decimal? giaDen, DateTime? ngayThem, int? loaiSanPham, string sortOrder, int page = 1, int pageSize = 5)
        {
            // Đánh dấu menu chính đang active
            ViewBag.ActiveMenu = "SanPham";

            // Xác định submenu active dựa vào bộ lọc
            if (!string.IsNullOrEmpty(sortOrder))
            {
                ViewBag.SubMenu = sortOrder == "name_asc" ? "SortNameAsc" :
                                  sortOrder == "name_desc" ? "SortNameDesc" :
                                  sortOrder == "date_desc" ? "SortDateDesc" : "AllProducts";
            }
            else if (giaTu == 4000000 && giaDen == 5000000)
            {
                ViewBag.SubMenu = "Price4to5";
            }
            else if (giaTu == 5000000 && giaDen == 6000000)
            {
                ViewBag.SubMenu = "Price5to6";
            }
            else if (giaTu == 6000000)
            {
                ViewBag.SubMenu = "PriceAbove6";
            }
            else
            {
                ViewBag.SubMenu = "AllProducts";
            }

            // Truy vấn danh sách sản phẩm
            var list = db.SanPhams.Include(s => s.LoaiSanPham).AsQueryable();

            if (!string.IsNullOrEmpty(tenSanPham))
            {
                list = list.Where(s => s.TenSanPham.Contains(tenSanPham));
            }

            if (giaTu.HasValue)
            {
                list = list.Where(s => (s.GiamGia > 0 ? s.GiaGiam : s.Gia) >= giaTu.Value);
            }

            if (giaDen.HasValue)
            {
                list = list.Where(s => (s.GiamGia > 0 ? s.GiaGiam : s.Gia) <= giaDen.Value);
            }

            if (ngayThem.HasValue)
            {
                DateTime date = ngayThem.Value.Date;
                list = list.Where(s => DbFunctions.TruncateTime(s.NgayThem) == date);
            }

            if (loaiSanPham.HasValue && loaiSanPham != 0)
            {
                list = list.Where(s => s.IDLoaiSp == loaiSanPham.Value);
            }

            switch (sortOrder)
            {
                case "date_desc":
                    list = list.OrderByDescending(s => s.NgayThem);
                    break;
                case "name_asc":
                    list = list.OrderBy(s => s.TenSanPham);
                    break;
                case "name_desc":
                    list = list.OrderByDescending(s => s.TenSanPham);
                    break;
                default:
                    list = list.OrderBy(s => s.NgayThem);
                    break;
            }

            int totalItems = list.Count();
            var pagedList = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TenSanPham = tenSanPham;
            ViewBag.GiaTu = giaTu;
            ViewBag.GiaDen = giaDen;
            ViewBag.NgayThem = ngayThem?.ToString("yyyy-MM-dd");
            ViewBag.LoaiSanPham = loaiSanPham;
            ViewBag.ListLoaiSanPham = new SelectList(db.LoaiSanPhams, "IDLoaiSp", "TenLoai");

            ViewBag.SortOrder = sortOrder;
            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPage = page;

            return View(pagedList);
        }

        // Add (GET)
        public ActionResult Add()
        {
            ViewBag.IDLoaiSp = new SelectList(db.LoaiSanPhams, "IDLoaiSp", "TenLoai");
            var model = new SanPham(); // Khởi tạo đối tượng SanPham rỗng
            return View(model); // Truyền vào View
        }
        // Add (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(SanPham sp, HttpPostedFileBase UploadImage)
        {
            if (ModelState.IsValid)
            {
                // Xử lý hình ảnh
                if (UploadImage != null && UploadImage.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(UploadImage.FileName);

                    // Xác định folder theo loại sản phẩm
                    string folderName = GetFolderName(sp.IDLoaiSp);
                    string folderPath = Server.MapPath("~/images/" + folderName + "/");

                    // Kiểm tra folder tồn tại, nếu chưa thì tạo
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Đường dẫn lưu file
                    string path = Path.Combine(folderPath, fileName);
                    UploadImage.SaveAs(path);

                    // Lưu đường dẫn vào DB
                    sp.HinhAnh = "~/images/" + folderName + "/" + fileName;
                }
      
                sp.NgayThem = DateTime.Now;
                db.SanPhams.Add(sp);
                db.SaveChanges();
                return RedirectToAction("Info");
            }

            ViewBag.IDLoaiSp = new SelectList(db.LoaiSanPhams, "IDLoaiSp", "TenLoai", sp.IDLoaiSp);
            return View(sp);
        }
        public ActionResult LoaiSanPham()
        {
            var loaiSanPhams = db.LoaiSanPhams.ToList();
            return View(loaiSanPhams);
        }

        // Thêm loại sản phẩm - GET
        public ActionResult AddLoaiSanPham()
        {
            return View();
        }

        // Thêm loại sản phẩm - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLoaiSanPham(LoaiSanPham loaiSanPham)
        {
            if (ModelState.IsValid)
            {
                db.LoaiSanPhams.Add(loaiSanPham);
                db.SaveChanges();
                return RedirectToAction("LoaiSanPham");
            }
            return View(loaiSanPham);
        }

        // Sửa loại sản phẩm - GET
        public ActionResult EditLoaiSanPham(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var loaiSanPham = db.LoaiSanPhams.Find(id);
            if (loaiSanPham == null) return HttpNotFound();
            return View(loaiSanPham);
        }

        // Sửa loại sản phẩm - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLoaiSanPham(LoaiSanPham loaiSanPham)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loaiSanPham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LoaiSanPham");
            }
            return View(loaiSanPham);
        }

        // Xóa loại sản phẩm
        public ActionResult DeleteLoaiSanPham(int id)
        {
            var loaiSanPham = db.LoaiSanPhams.Find(id);
            if (loaiSanPham != null)
            {
                db.LoaiSanPhams.Remove(loaiSanPham);
                db.SaveChanges();
            }
            return RedirectToAction("LoaiSanPham");
        }

        // Edit (GET)
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            SanPham sp = db.SanPhams.Find(id);
            if (sp == null) return HttpNotFound();

            ViewBag.IDLoaiSp = new SelectList(db.LoaiSanPhams, "IDLoaiSp", "TenLoai", sp.IDLoaiSp);
            return View(sp);
        }

        // Edit (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SanPham sp, HttpPostedFileBase UploadImage)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = db.SanPhams.AsNoTracking().FirstOrDefault(x => x.IDLoaiSp == sp.IDLoaiSp);
                if (existingProduct == null) return HttpNotFound();

                // Nếu có upload ảnh mới → Xóa ảnh cũ, lưu ảnh mới
                if (UploadImage != null && UploadImage.ContentLength > 0)
                {
                    // Xóa ảnh cũ
                    if (!string.IsNullOrEmpty(existingProduct.HinhAnh))
                    {
                        string oldPath = Server.MapPath(existingProduct.HinhAnh);
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    string fileName = Path.GetFileName(UploadImage.FileName);
                    string folderName = GetFolderName(sp.IDLoaiSp);
                    string folderPath = Server.MapPath("~/images/" + folderName + "/");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string path = Path.Combine(folderPath, fileName);
                    UploadImage.SaveAs(path);

                    sp.HinhAnh = "~/images/" + folderName + "/" + fileName;
                }
                else
                {
                    // Không upload ảnh mới → giữ nguyên ảnh cũ
                    sp.HinhAnh = existingProduct.HinhAnh;
                }
                

                sp.NgayThem = DateTime.Now;
                db.Entry(sp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Info");
            }

            ViewBag.IDLoaiSp = new SelectList(db.LoaiSanPhams, "IDLoaiSp", "TenLoai", sp.IDLoaiSp);
            return View(sp);
        }

        // Delete (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProduct(int id)
        {
            var sp = db.SanPhams.Find(id);
            if (sp == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
            }

            // Xóa file ảnh
            if (!string.IsNullOrEmpty(sp.HinhAnh))
            {
                string fullPath = Server.MapPath(sp.HinhAnh);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            string tenSanPham = sp.TenSanPham;
            db.SanPhams.Remove(sp);
            db.SaveChanges();

            return Json(new { success = true, message = $"Đã xóa sản phẩm {tenSanPham} thành công" });
        }


        // Hàm xác định folder lưu ảnh theo loại sản phẩm
        private string GetFolderName(int? idLoaiSp)
        {
            string folder = "others";

            if (idLoaiSp == 1) folder = "trangsucCuoi";
            else if (idLoaiSp == 2) folder = "dongho";
            // Bạn có thể mở rộng thêm loại khác tại đây nếu muốn

            return folder;
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.ListLoaiSanPham = new SelectList(db.LoaiSanPhams, "IDLoaiSp", "TenLoai");
            base.OnActionExecuting(filterContext);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
