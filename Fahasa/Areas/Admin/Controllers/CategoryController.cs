using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CloudinaryDotNet.Actions;
using Fahasa.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using static Fahasa.FilterConfig;

namespace Fahasa.Areas.Admin.Controllers
{
    [RedirectingAction]
    public class CategoryController : Controller
    {
        BookManagementEntities db = new BookManagementEntities();
        public ActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult GetCategories()
        {
            try
            {
                return Json(db.Categories.Select(x => new { x.Id, x.Name }).ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        public JsonResult DsCategory()
        {
            try
            {
                var dsCategory = db.Categories.Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

                return Json(new { data = dsCategory }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { code = 500, msg = "Lấy danh sách thất bại: " + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        public ActionResult AddCategory(string values)
        {
            try
            {
                var category = new Category();
                JsonConvert.PopulateObject(values, category);

                if (!TryValidateModel(category)) {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                category = db.Categories.Add(category);
                db.SaveChanges();

                return Json(new {
                    Id = category.Id,
                    Name = category.Name
                }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                Response.StatusCode = 400;
                return Json(new { code = 500, msg = "Lỗi: "+ ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UploadExcel(HttpPostedFileBase file)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        var worksheet = package.Workbook.Worksheets[0];

                        int rowCount = worksheet.Dimension.Rows;
                        int colCount = worksheet.Dimension.Columns;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var category = new Category();
                            string cateName = worksheet.Cells[row, 1].Text.Trim();
                            if (cateName != null && cateName != "")
                            {
                                category.Name = cateName;
                                db.Categories.Add(category);
                            }
                        }

                        db.SaveChanges();
                    }
                }

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPut]
        public ActionResult Update(int key, string values)
        {
            try
            {
                var category = db.Categories.FirstOrDefault(o => o.Id == key);
                if (category == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);

                JsonConvert.PopulateObject(values, category);

                if (!TryValidateModel(category))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                db.SaveChanges();
                return Json(new { code = 200, msg = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { code = 500, msg = "Cập nhật thất bại!" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpDelete]
        public ActionResult Delete(int key)
        {
            try
            {
                var category = db.Categories.FirstOrDefault(model => model.Id == key);

                if (category == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);

                db.Categories.Remove(category);
                db.SaveChanges();
                return Json(new { code = 200, msg = "Xóa thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { code = 500, msg = "Xóa thất bại! " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}