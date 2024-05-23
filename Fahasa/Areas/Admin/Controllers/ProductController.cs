using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Drawing;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Diagnostics;
using Fahasa.Models;
using static Fahasa.FilterConfig;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Security.Policy;
using OfficeOpenXml;
using System.Linq.Expressions;
using Fahasa.Ultis;
using Fahasa.Controllers;

namespace Fahasa.Areas.Admin.Controllers
{

    [RedirectingAction]
    public class ProductController : Controller
    {
        BookManagementEntities db = new BookManagementEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Load(string filter, int skip = 0, int take = 20)
        
        {
            try
            {
                IQueryable<Product> store = db.Products.AsQueryable();

                if (filter != null)
                {
                    var filterObjects = Fahasa.Ultis.Filter.Convert(filter);

                    var parameter = System.Linq.Expressions.Expression.Parameter(typeof(Product), "p");
                    Expression<Func<Product, bool>> expr = null;

                    for (int i = 0; i < filterObjects.Count; i += 2)
                    {
                        var condition = ExpressionFilter.Build<Product>(parameter, filterObjects[i]);
                        if (expr == null)
                            expr = condition;
                        else if ((string)filterObjects[i - 1] == "or")
                            expr = System.Linq.Expressions.Expression.Lambda<Func<Product, bool>>(System.Linq.Expressions.Expression.Or(expr.Body, condition.Body), parameter);
                        else
                            expr = System.Linq.Expressions.Expression.Lambda<Func<Product, bool>>(System.Linq.Expressions.Expression.And(expr.Body, condition.Body), parameter);
                    }

                    store = db.Products.Where(expr);
                }

                var products = store
                    .OrderByDescending(model => model.Id)
                    .Select(x => new
                {
                    Id = x.Id,
                    ImageSrc = x.ImageSrc,
                    Name = x.Name,
                    Price = x.Price,
                    SoonRelease = x.SoonRelease,
                    StockAvailable = x.StockAvailable,
                    SupplierId = x.SupplierId,
                    AuthorId = x.AuthorId,
                    CoverId = x.CoverId,
                    Amount = x.Amount,
                    Discount = x.Discount,
                    PublisherId = x.PublisherId,
                    Brand = x.Brand,
                    Origin = x.Origin,
                    x.Desc,
                    x.Table,
                    Categories = x.Categories.Select(c => new { Id = c.Id, Name = c.Name }).ToList(),
                    Galleries = x.Galleries.Select(g => new { Source = g.Source }).ToList()
                });

                var count = products.Count();

                var result = Json(new { data = products.Skip(skip).Take(take), totalCount = count }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet)
                ;
                result.MaxJsonLength = Int32.MaxValue;
                return result;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                return Json(ex.Message, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Add(string values)
        {
            try
            {
                var product = new Product();
                product.Discount = 0;

                JsonConvert.PopulateObject(values, product);
                var categories = new List<Category>();
                product.StockAvailable = product.Amount > 0 ? "in_stock" : "out_of_stock";

                product.Categories.ToList().ForEach(c =>
                {
                    var category = db.Categories.FirstOrDefault(c2 => c2.Id == c.Id);
                    if (category != null) categories.Add(category);
                });

                product.Categories = categories;
                db.Products.Add(product);
                db.SaveChanges();

                return Json(new
                {
                    Id = product.Id,
                    Name = product.Name
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
                            var product = new Product();
                            List<string> categoryIds = new List<string>();
                            var categories = new List<Category>();
                            product.ImageSrc = worksheet.Cells[row, 1].Text;
                            product.Name = worksheet.Cells[row, 2].Text;
                            product.Price = Int32.Parse(worksheet.Cells[row, 3].Text);
                            product.Amount = Int32.Parse(worksheet.Cells[row, 4].Text);
                            product.Discount = Int32.Parse(worksheet.Cells[row, 5].Text);
                            product.Brand = worksheet.Cells[row, 6].Text;
                            product.Origin = worksheet.Cells[row, 7].Text;
                            product.SoonRelease = false;
                            string supplierName = worksheet.Cells[row, 10].Text.Trim();
                            string authorName = worksheet.Cells[row, 11].Text.Trim();
                            string coverName = worksheet.Cells[row, 12].Text.Trim();
                            string publisherName = worksheet.Cells[row, 13].Text.Trim();
                            if (supplierName != null && supplierName != "")
                            {
                                var supplier = db.Suppliers.FirstOrDefault(x => x.Name == supplierName);
                                if (supplier != null) {
                                    product.SupplierId = supplier.Id;
                                }
                            }
                            if (authorName != null && authorName != "")
                            {
                                var author = db.Authors.FirstOrDefault(x => x.Name == authorName);
                                if (author != null)
                                {
                                    product.AuthorId = author.Id;
                                }
                            }
                            if (coverName != null && coverName != "")
                            {
                                var cover = db.Covers.FirstOrDefault(x => x.Name == coverName);
                                if (cover != null)
                                {
                                    product.CoverId = cover.Id;
                                }
                            }
                            if (publisherName != null && publisherName != "")
                            {
                                var publisher = db.Publishers.FirstOrDefault(x => x.Name == publisherName);
                                if (publisher != null)
                                {
                                    product.PublisherId = publisher.Id;
                                }
                            }

                            categoryIds =  worksheet.Cells[row, 9].Text.Split(',').ToList();

                            categoryIds.ForEach(c =>
                            {
                                var category = db.Categories.FirstOrDefault(c2 => c2.Name == c);
                                if (category != null) categories.Add(category);
                            });
                            product.Categories = categories;

                            db.Products.Add(product);
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

        [HttpPost]
        public ActionResult SaveTable(int Id, string Table)
        {
            try
            {
                var product = db.Products.FirstOrDefault(x => x.Id == Id);
                product.Table = Table;
                db.SaveChanges();

                return Json(new
                {
                    Id = product.Id,
                    Name = product.Name
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult SaveDesc(int Id, string Desc)
        {
            try
            {
                var product = db.Products.FirstOrDefault(x => x.Id == Id);
                product.Desc = Desc;
                db.SaveChanges();

                return Json(new
                {
                    Id = product.Id,
                    Name = product.Name
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPut]
        public ActionResult Update(int key, string values)
        {
            try
            {
                var product = db.Products.First(p => p.Id == key);

                JsonConvert.PopulateObject(values, product);
                var categories = new List<Category>();

                product.Categories.ToList().ForEach(c =>
                {
                    var category = db.Categories.FirstOrDefault(c2 => c2.Id == c.Id);
                    if (category != null) categories.Add(category);
                });

                product.Categories = categories;

                db.SaveChanges();

                return Json(new
                {
                    Id = product.Id,
                    Name = product.Name
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpDelete]
        public ActionResult Delete(int key)
        {
            try
            {
                var product = db.Products.First(p => p.Id == key);
                db.Products.Remove(product);
                db.SaveChanges();
                return Json(new
                {
                    Id = product.Id,
                    Name = product.Name
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult AddGallery(int Id, string Source)
        {
            try
            {
                Gallery gallery = new Gallery();
                gallery.ProductId = Id;
                gallery.Source = Source;

                db.Galleries.Add(gallery);
                db.SaveChanges();
                return Json(new
                {
                    Id = gallery.ProductId,
                    Name = gallery.Source
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult DeleteGallery(int Id, string Source)
        {
            try
            {
                CloudinaryBase.DeleteImage(Source);

                db.Galleries.Remove(db.Galleries.FirstOrDefault(g => g.ProductId == Id));
                db.SaveChanges();
                return Json(new
                {
                    Id = Id,
                    Name = Source
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
    }
}