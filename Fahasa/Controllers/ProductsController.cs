using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Fahasa.Models;
using Fahasa.Ultis;
using static Fahasa.FilterConfig;

namespace Fahasa.Controllers
{
    [RefreshAction]
    public class ProductsController : Controller
    {
        private BookManagementEntities db = new BookManagementEntities();

        public ActionResult Details(int id)
        {
            try
            {
                dynamic model = new ExpandoObject();
                model.Product = db.Products.FirstOrDefault(x => x.Id == id);

                List<Discount> discounts = new List<Discount>();

                foreach (var item in db.Discounts.ToList())
                {
                    if (DiscountUltis.checkDiscount(item, id))
                    {
                        discounts.Add(item);
                    }
                    if (discounts.Count == 3)
                        break;
                }

                model.Discounts = db.Discounts.ToList().FindAll(x => DiscountUltis.checkDiscount(x, new List<int> { id }) && DiscountUltis.checkApply((long)PriceUltis.get(model.Product.Price, model.Product.Discount), x.ConditionalOperator, x.ConditionalPrice)).ToList();
                var comments = db.Comments.Where(c => c.ProductId == id).ToList();

                var starStatistics = new List<double>();
                for (int i = 5; i >= 1; i--)
                {
                    double percentStar = comments.Where(c => c.Rating == i).Count() / Math.Max(comments.Count() * 1.0, 1.0) * 100;
                    starStatistics.Add(percentStar);
                }

                model.comments = comments;
                model.starStatistics = starStatistics;
                return View(model);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult GetAllDiscount(int pId)
        {
            try
            {
                var product = db.Products.FirstOrDefault(x => x.Id == pId);
                var list = db.Discounts.ToList().FindAll(x => DiscountUltis.checkDiscount(x, new List<int> { pId }) && DiscountUltis.checkApply((long)PriceUltis.get(product.Price, product.Discount), x.ConditionalOperator, x.ConditionalPrice)).ToList();

                return Json(list.Select(x => new
                {
                    x.Code,
                    x.Title,
                    x.Value,
                    x.Type,
                    x.DateExpired,
                    x.Amount,
                    x.DateStart,
                    x.ApplyField,
                    x.ConditionalOperator,
                    x.ConditionalPrice,
                    x.Content,
                    x.Used,
                    x.SubTitle,
                    x.LimitUsed
                }).ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult GetDiscountDetail(string Id)
        {
            try
            {
                var x = db.Discounts.ToList().First(d => d.Code == Id);

                return Json(new
                {
                    x.Code,
                    x.Title,
                    x.Value,
                    x.Type,
                    x.DateExpired,
                    x.Amount,
                    x.DateStart,
                    x.ApplyField,
                    x.ConditionalOperator,
                    x.ConditionalPrice,
                    x.Content,
                    x.Used,
                    x.SubTitle,
                    x.LimitUsed
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult Search(string cate = "", string q = "")
        {
            try
            {
                IQueryable<Product> productQuery = null;

                if (cate != "")
                {
                    productQuery = db.Products.Where(p => p.Categories.Any(c => c.Name.ToUpper().Contains(cate.ToUpper())));
                }
                else if (q != "")
                {
                    productQuery = db.Products.Where(p => p.Name.ToUpper().Contains(q.ToUpper()));
                }
                else
                {
                    productQuery = db.Products.AsQueryable();
                }

                var products = productQuery.OrderByDescending(x => x.Id).Skip(0).Take(48).Select(x => new ProductViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    Brand = x.Brand,
                    Origin = x.Origin,
                    ImageSrc = x.ImageSrc,
                    Discount = x.Discount,
                    Rating = db.Comments.AsQueryable().Where(c => c.ProductId == x.Id).Average(c2 => c2.Rating),
                    RatingCount = db.Comments.AsQueryable().Where(c => c.ProductId == x.Id).Count(),
                    AuthorId = x.AuthorId,
                    SupplierId = x.SupplierId,
                    PublisherId = x.PublisherId,
                    CoverId = x.CoverId,
                }
                ).ToList();
                var count = productQuery.Count();

                ViewData["count"] = count;
                ViewData["products"] = products;
                var authorIds = products.Select(x => x.AuthorId).ToList();
                var supplierIds = products.Select(x => x.SupplierId).ToList();
                var publisherIds = products.Select(x => x.PublisherId).ToList();
                var coverIds = products.Select(x => x.CoverId).ToList();

                var authors = db.Authors.Where(x => authorIds.Contains(x.Id)).ToList();
                var covers = db.Covers.Where(x => coverIds.Contains(x.Id)).ToList();
                var suppliers = db.Suppliers.Where(x => supplierIds.Contains(x.Id)).ToList();
                var publishers = db.Publishers.Where(x => publisherIds.Contains(x.Id)).ToList();
                var categories = products.Select(x => x.Categories.Select(y => y)).ToList();
                var brands = products.Select(x => x.Brand).Distinct().Where(x => x != null).ToList();
                var origins = products.Select(x => x.Origin).Distinct().Where(x => x != null).ToList();
                ViewData["origins"] = origins;
                ViewData["brands"] = brands;
                ViewData["categories"] = categories;
                ViewData["publishers"] = publishers;
                ViewData["suppliers"] = suppliers;
                ViewData["covers"] = covers;
                ViewData["authors"] = authors;

                var totalPages = Math.Ceiling((decimal)(count / Constanst.SEARCH_TAKE_DEFAULT));
                var firstPage = Math.Max(1, 1 - Constanst.SEARCH_MAX_DISTANCE);
                var lastPage = Math.Min(totalPages, 1 + Constanst.SEARCH_MAX_DISTANCE);

                ViewData["first"] = firstPage;
                ViewData["last"] = lastPage;
                ViewData["current"] = 1;
                ViewData["q"] = q;

                ViewData["prices"] = new List<FilterPrice>() {
                new FilterPrice(){ from = 0, to = 150000, text = "0đ - 150,000đ" },
                new FilterPrice(){ from = 150000, to = 300000, text = "150,000đ - 300,000đ" },
                new FilterPrice(){ from = 300000, to = 500000, text = "300,000đ - 500,000đ" },
                new FilterPrice(){ from = 500000, to = 700000, text = "500,000đ - 700,000đ" },
                new FilterPrice(){ from = 700000, to = 50000000, text = "700,000đ trở lên" },
                new FilterPrice(){ from = 0, to = 50000000, text = "Tất cả" },
            };

                return View();
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult Search(Models.Filter filter, string q = "")
        {
            try
            {
                float take = Constanst.SEARCH_TAKE_DEFAULT;
                IQueryable<Product> proQuery = db.Products.Where(x => x.Name.ToUpper().Contains(q.ToUpper()) || q.Equals("") || q == null);

                if (filter.brand != null && filter.brand.value != null && filter.brand.value.Count > 0 && filter.brand.type == "any")
                {
                    proQuery = proQuery.Where(x => filter.brand.value.Contains(x.Brand));
                }

                if (filter.categories != null && filter.categories.value != null && filter.categories.value.Count > 0 && filter.categories.type == "any")
                {
                    proQuery = proQuery.Where(x => x.Categories.Any(c => filter.categories.value.Contains(c.Name)));
                }


                if (filter.publisher != null && filter.publisher.value != null && filter.publisher.value.Count > 0 && filter.publisher.type == "any")
                {
                    proQuery = proQuery.Where(x => filter.publisher.value.Contains(x.PublisherId ?? 0));
                }

                if (filter.author != null && filter.author.value != null && filter.author.value.Count > 0 && filter.author.type == "any")
                {
                    proQuery = proQuery.Where(x => filter.author.value.Contains(x.AuthorId ?? 0));
                }

                if (filter.supplier != null && filter.supplier.value != null && filter.supplier.value.Count > 0 && filter.supplier.type == "any")
                {
                    proQuery = proQuery.Where(x => filter.supplier.value.Contains(x.SupplierId ?? 0));
                }

                if (filter.cover != null && filter.cover.value != null && filter.cover.value.Count > 0 && filter.cover.type == "any")
                {
                    proQuery = proQuery.Where(x => filter.cover.value.Contains(x.CoverId ?? 0));
                }

                if (filter.origin != null && filter.origin.value != null && filter.origin.value.Count > 0 && filter.origin.type == "any")
                {
                    proQuery = proQuery.Where(x => filter.origin.value.Contains(x.Origin));
                }

                if (filter.price != null && filter.price.value != null && filter.price.type == "any")
                {
                    var filterPrices = new List<long>();
                    filterPrices.Add(filter.price.value.to);
                    filterPrices.Add(filter.price.value.from);

                    var filterPriceMax = filterPrices.Max();
                    var filterPriceMin = filterPrices.Min();

                    proQuery = proQuery.Where(x => (long)Math.Floor(x.Price * (1 - (x.Discount ?? 0) / 100.0) / 100) * 100 >= filterPriceMin && (long)Math.Floor(x.Price * (1 - (x.Discount ?? 0) / 100.0) / 100) * 100 <= filterPriceMax);
                }

                if (filter.take != null)
                {
                    take = filter.take.value;
                }

                if (filter.sort != null)
                {
                    if (filter.sort.value.field == "price" && filter.sort.value.direction == "desc")
                    {
                        proQuery = proQuery.OrderByDescending(x => (long)Math.Floor(x.Price * (1 - (x.Discount ?? 0) / 100.0) / 100) * 100);
                    }

                    if (filter.sort.value.field == "price" && filter.sort.value.direction == "asc")
                    {
                        proQuery = proQuery.OrderBy(x => (long)Math.Floor(x.Price * (1 - (x.Discount ?? 0) / 100.0) / 100) * 100);
                    }

                    if (filter.sort.value.field == "id" && filter.sort.value.direction == "desc")
                    {
                        proQuery = proQuery.OrderByDescending(x => x.Id);
                    }

                    if (filter.sort.value.field == "discount" && filter.sort.value.direction == "desc")
                    {
                        proQuery = proQuery.OrderByDescending(x => x.Discount);
                    }
                }
                else
                {
                    proQuery = proQuery.OrderBy(x => x.Id);
                }

                if (filter.page == null)
                {
                    filter.page = new FilterItem<int>();
                    filter.page.value = 1;
                }

                var count = proQuery.Count();
                var totalPages = Math.Ceiling((decimal)(count / take));
                var firstPage = Math.Max(1, filter.page.value - Constanst.SEARCH_MAX_DISTANCE);
                var lastPage = Math.Min(totalPages, filter.page.value + Constanst.SEARCH_MAX_DISTANCE);

                var products = proQuery.AsQueryable().Select(x => new ProductViewModel
                {
                    Name = x.Name,
                    Id = x.Id,
                    Price = x.Price,
                    Brand = x.Brand,
                    Origin = x.Origin,
                    ImageSrc = x.ImageSrc,
                    Discount = x.Discount,
                    Rating = db.Comments.AsQueryable().Where(c => c.ProductId == x.Id).Average(c2 => c2.Rating),
                    RatingCount = db.Comments.AsQueryable().Where(c => c.ProductId == x.Id).Count(),
                }
                ).Skip((int)((filter.page.value - 1) * take)).Take(((int)take)).ToList();

                return Json(new { products = products, q = q, count = count, page = new { first = firstPage, last = lastPage, current = filter.page.value } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
    }
}
