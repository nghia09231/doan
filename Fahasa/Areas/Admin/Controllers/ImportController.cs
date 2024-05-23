using CloudinaryDotNet;
using ExcelDataReader;
using Fahasa.Controllers;
using Fahasa.Models;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static Fahasa.FilterConfig;

namespace Fahasa.Areas.Admin.Controllers
{
    //[RedirectingAction]
    public class ImportController : Controller
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

        public string Get()
        {
            try
            {
                return JsonConvert.SerializeObject(db.Imports.Select(x => new
            {
                x.Id,
                x.StaffId,
                x.ImportDate,
                x.Note,
            }).ToList());
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return JsonConvert.SerializeObject(null);
            }
        }

        [HttpPost]
        public ActionResult Add(string values)
        {
            try
            {
                var import = new Import();
                JsonConvert.PopulateObject(values, import);

                if (!TryValidateModel(import))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                import = db.Imports.Add(import);
                db.SaveChanges();

                return Json(new
                {
                    Id = import.Id,
                    ImportDate = import.ImportDate
                }, JsonRequestBehavior.AllowGet);
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
                var import = db.Imports.FirstOrDefault(o => o.Id == key);
                if (import == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);

                JsonConvert.PopulateObject(values, import);

                if (!TryValidateModel(import))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                db.SaveChanges();
                return Json(new { code = 200, msg = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                return Json(new { code = 500, msg = "Cập nhật thất bại!" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete]
        public ActionResult Delete(int key)
        {
            try
            {
                var import = db.Imports.FirstOrDefault(o => o.Id == key);

                if (import == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);

                db.Imports.Remove(import);
                db.SaveChanges();
                return Json(new { code = 200, msg = "Xóa thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                return Json(new { code = 500, msg = "Xóa thất bại! " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public string GetProducts(int ImportId) {
            return JsonConvert.SerializeObject(db.ImportProducts.Where(ip => ip.ImportId == ImportId)
                    .Select(ip => new
                    {
                        ProductId = ip.Product.Id,
                        ip.Product.Name,
                        ip.Product.ImageSrc,
                        ip.Amount,
                        ip.Price
                    }).ToList());
        }

        [HttpPost]
        public ActionResult AddProduct(int ImportId, string values)
        {
            try
            {
                var importProduct = new ImportProduct();
                JsonConvert.PopulateObject(values, importProduct);

                if (!TryValidateModel(importProduct))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                importProduct.ImportId = ImportId;
                importProduct = db.ImportProducts.Add(importProduct);
                db.SaveChanges();


                var product = db.Products.First(p => p.Id == importProduct.ProductId);
                product.Amount += importProduct.Amount;
                db.SaveChanges();

                return Json(new
                {
                    Id = importProduct.ImportId,
                    Name = importProduct.ProductId
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete]
        public ActionResult DeleteProduct(int ImportId, int key)
        {
            try
            {
                var importProduct = db.ImportProducts.First(ip => ip.ImportId == ImportId && ip.ProductId == key);
                db.ImportProducts.Remove(importProduct);
                db.SaveChanges();

                var product = db.Products.First(p => p.Id == importProduct.ProductId);
                product.Amount -= importProduct.Amount;
                db.SaveChanges();
                return Json(new { code = 200, msg = "Xóa thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                return Json(new { code = 500, msg = "Xóa thất bại! " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UploadProductExcel(int ImportId, HttpPostedFileBase file)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    // Đọc dữ liệu từ tệp Excel

                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        var worksheet = package.Workbook.Worksheets[0];

                        int rowCount = worksheet.Dimension.Rows;
                        int colCount = worksheet.Dimension.Columns;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var rowData = new ImportProduct();

                            rowData.ImportId = ImportId;
                            rowData.ProductId = Int32.Parse(worksheet.Cells[row, 1].Text);
                            rowData.Price = Int32.Parse(worksheet.Cells[row, 2].Text);
                            rowData.Amount = Int32.Parse(worksheet.Cells[row, 3].Text);

                            db.ImportProducts.Add(rowData);
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
    }
}