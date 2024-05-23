using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using Fahasa.Models;
using Microsoft.Ajax.Utilities;
using WebGrease.Css.Extensions;
using System.Dynamic;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using static Fahasa.FilterConfig;
using System.Net;
using Fahasa.Ultis;

namespace Fahasa.Controllers
{
    [RefreshAction]
    public class HomeController : Controller
    {
        BookManagementEntities db = new BookManagementEntities();

        public ActionResult Index()
        {
            try
            {
                dynamic model = new ExpandoObject();

                model.Literatures = db.Products.Where(x => x.Categories.FirstOrDefault(y => y.Name == "Văn học") != null && x.Comments.Average(c => c.Rating) > 4.5).OrderByDescending(x => x.Id).Take(10).Select(x => new ProductViewModel { Name = x.Name, Id = x.Id, Price = x.Price, Brand = x.Brand, Origin = x.Origin, ImageSrc = x.ImageSrc, Discount = x.Discount, Rating = x.Comments.Average(p => p.Rating), RatingCount = x.Comments.Count() }).ToList();
                model.Stationeries = db.Products.Where(x => x.Categories.FirstOrDefault(y => y.Name == "Văn phòng phẩm - Dụng Cụ Học Sinh") != null && x.Comments.Average(c => c.Rating) > 4.5).OrderByDescending(x => x.Id).Take(10).Select(x => new ProductViewModel { Name = x.Name, Id = x.Id, Price = x.Price, Brand = x.Brand, Origin = x.Origin, ImageSrc = x.ImageSrc, Discount = x.Discount, Rating = x.Comments.Average(p => p.Rating), RatingCount = x.Comments.Count() }).ToList();
                model.ScienceTechnology = db.Products.Where(x => x.Categories.FirstOrDefault(y => y.Name == "Khoa học kỹ thuật") != null && x.Comments.Average(c => c.Rating) > 4.5).OrderByDescending(x => x.Id).Take(10).Select(x => new ProductViewModel { Name = x.Name, Id = x.Id, Price = x.Price, Brand = x.Brand, Origin = x.Origin, ImageSrc = x.ImageSrc, Discount = x.Discount, Rating = x.Comments.Average(p => p.Rating), RatingCount = x.Comments.Count() }).ToList();
                model.ReferenceBooks = db.Products.Where(x => x.Categories.FirstOrDefault(y => y.Name == "Giáo khoa - Tham khảo") != null && x.Comments.Average(c => c.Rating) > 4.5).OrderByDescending(x => x.Id).Take(10).Select(x => new ProductViewModel { Name = x.Name, Id = x.Id, Price = x.Price, Brand = x.Brand, Origin = x.Origin, ImageSrc = x.ImageSrc, Discount = x.Discount, Rating = x.Comments.Average(p => p.Rating), RatingCount = x.Comments.Count() }).ToList();
                model.SchoolSupplies = db.Products.Where(x => x.Categories.FirstOrDefault(y => y.Name == "Dụng cụ học sinh") != null && x.Comments.Average(c => c.Rating) > 4.5).OrderByDescending(x => x.Id).Take(10).Select(x => new ProductViewModel { Name = x.Name, Id = x.Id, Price = x.Price, Brand = x.Brand, Origin = x.Origin, ImageSrc = x.ImageSrc, Discount = x.Discount, Rating = x.Comments.Average(p => p.Rating), RatingCount = x.Comments.Count() }).ToList();
                model.Economic = db.Products.Where(x => x.Categories.FirstOrDefault(y => y.Name == "Kinh tế") != null && x.Comments.Average(c => c.Rating) > 4.5).OrderByDescending(x => x.Id).Take(10).Select(x => new ProductViewModel { Name = x.Name, Id = x.Id, Price = x.Price, Brand = x.Brand, Origin = x.Origin, ImageSrc = x.ImageSrc, Discount = x.Discount, Rating = x.Comments.Average(p => p.Rating), RatingCount = x.Comments.Count() }).ToList();
                model.ForeignBooks = db.Products.Where(x => x.Categories.FirstOrDefault(y => y.Name == "Foreign books" && x.Comments.Average(c => c.Rating) > 4.5) != null).OrderByDescending(x => x.Id).Take(10).Select(x => new ProductViewModel { Name = x.Name, Id = x.Id, Price = x.Price, Brand = x.Brand, Origin = x.Origin, ImageSrc = x.ImageSrc, Discount = x.Discount, Rating = x.Comments.Average(p => p.Rating), RatingCount = x.Comments.Count() }).ToList();
                model.OtherLanguages = db.Products.Where(x => x.Categories.FirstOrDefault(y => y.Name == "Other languages" && x.Comments.Average(c => c.Rating) > 4.5) != null).OrderByDescending(x => x.Id).Take(10).Select(x => new ProductViewModel { Name = x.Name, Id = x.Id, Price = x.Price, Brand = x.Brand, Origin = x.Origin, ImageSrc = x.ImageSrc, Discount = x.Discount, Rating = x.Comments.Average(p => p.Rating), RatingCount = x.Comments.Count() }).ToList();
                model.Toys = db.Products.Where(x => x.Categories.FirstOrDefault(y => y.Name == "Đồ Chơi") != null && x.Comments.Average(c => c.Rating) > 4.5).OrderByDescending(x => x.Id).Take(10).Select(x => new ProductViewModel { Name = x.Name, Id = x.Id, Price = x.Price, Brand = x.Brand, Origin = x.Origin, ImageSrc = x.ImageSrc, Discount = x.Discount, Rating = x.Comments.Average(p => p.Rating), RatingCount = x.Comments.Count() }).ToList();


                return View(model);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
    }
}