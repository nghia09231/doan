using Fahasa.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static Fahasa.FilterConfig;

namespace Fahasa.Areas.Admin.Controllers
{
    [RedirectingAction]
    public class DiscountController : Controller
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

        public string GetDiscounts()
        {
            try
            {
                return JsonConvert.SerializeObject(db.Discounts.Select(x => new { x.Code, x.Title, x.Value, x.Type, x.DateExpired, x.Amount, x.DateStart, x.ApplyField, x.ConditionalOperator, x.ConditionalPrice, x.Content, x.Used, x.SubTitle, x.LimitUsed }).ToList());
            }
            catch (Exception)
            {
                Response.StatusCode = 400;
                return JsonConvert.SerializeObject(null);
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Add(string values)
        {
            try
            {
                var discount = new Discount();
                JsonConvert.PopulateObject(values, discount);

                if (!TryValidateModel(discount))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (discount.Used == null)
                {
                    discount.Used = 0;
                }
                discount = db.Discounts.Add(discount);
                db.SaveChanges();

                return Json(new
                {
                    Code = discount.Code,
                    Title = discount.Title
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
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
                            var discount = new Discount();
                            if (discount.Used == null)
                            {
                                discount.Used = 0;
                            }
                            discount.Code = worksheet.Cells[row, 1].Text.Trim();
                            discount.Title = worksheet.Cells[row, 2].Text.Trim();
                            discount.SubTitle = worksheet.Cells[row, 3].Text.Trim();
                            discount.LimitUsed = int.Parse(worksheet.Cells[row, 4].Text.Trim());
                            discount.DateStart = DateTime.ParseExact(worksheet.Cells[row, 5].Text.Trim(), "yyyy-MM-ddTHH:mm:ss",
                                        CultureInfo.InvariantCulture);
                            discount.DateExpired = DateTime.ParseExact(worksheet.Cells[row, 6].Text.Trim(), "yyyy-MM-ddTHH:mm:ss",
                                        CultureInfo.InvariantCulture);
                            discount.Value = int.Parse(worksheet.Cells[row, 7].Text.Trim());
                            discount.Type = worksheet.Cells[row, 8].Text.Trim();
                            discount.Amount = int.Parse(worksheet.Cells[row, 9].Text.Trim());
                            discount.ApplyField = worksheet.Cells[row, 10].Text.Trim();
                            discount.ConditionalOperator = worksheet.Cells[row, 11].Text.Trim();
                            discount.ConditionalPrice = int.Parse(worksheet.Cells[row, 12].Text.Trim());
                            discount.Content = worksheet.Cells[row, 13].Text.Trim();
                            db.Discounts.Add(discount);
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

        [HttpPut, ValidateInput(false)]
        public ActionResult Update(string key, string values)
        {
            try
            {
                var discount = db.Discounts.FirstOrDefault(o => o.Code == key);
                if (discount == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);

                JsonConvert.PopulateObject(values, discount);

                if (!TryValidateModel(discount))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                db.SaveChanges();
                return Json(new { code = 200, msg = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { code = 500, msg = "Cập nhật thất bại!" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete]
        public ActionResult Delete(string key)
        {
            try
            {
                var discount = db.Discounts.FirstOrDefault(model => model.Code == key);

                if (discount == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);

                db.Discounts.Remove(discount);
                db.SaveChanges();
                return Json(new { code = 200, msg = "Xóa thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { code = 500, msg = "Xóa thất bại! " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}