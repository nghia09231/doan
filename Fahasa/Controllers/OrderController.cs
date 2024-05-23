using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using CloudinaryDotNet.Actions;
using Fahasa.Models;
using Fahasa.Ultis;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using WebGrease.Css.Extensions;
using static Fahasa.FilterConfig;
using static OfficeOpenXml.ExcelErrorValue;

namespace Fahasa.Controllers
{
    [RefreshAction]
    public class OrderController : Controller
    {
        private BookManagementEntities db = new BookManagementEntities();

        // GET: CartItems
        public ActionResult Index()
        {
            try
            {
                dynamic model = new ExpandoObject();

                CartTemp cart = getCart();
                cart.FeeShip = 0;
                cart.CartItems.ForEach(x => x.IsChecked = false);
                model = cart;
                return View(model);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public CartTemp getCart() // Create list cart and save in session 
        {
            CartTemp cart = Session["Cart"] as CartTemp;
            var people = Session["info"] as Person;

            if (cart == null)
            {
                cart = new CartTemp();
                if (people != null)
                {
                    var cartDetail = db.Carts.Where(x => x.UserId == people.Id).ToList();
                    cartDetail.ForEach(item =>
                    {
                        var cartItem = new CartItem();
                        cartItem.Product = item.Product;
                        cartItem.Amount = item.Amount ?? 1;
                        cartItem.Price = PriceUltis.get(cartItem.Product.Price, cartItem.Product.Discount ?? 0);
                        cartItem.TotalPrice = cartItem.Price * cartItem.Amount;
                        cart.CartItems.Add(cartItem);
                    });
                    cart.CartItems.Reverse();
                }
            }

            Session["Cart"] = cart;
            return cart;
        }

        public ActionResult AddToCart(int id, int qty) //  Add item in cart 
        {
            try
            {
                var people = Session["info"] as Person;
                CartTemp cart = getCart();
                CartItem item = cart.CartItems.Find(model => model.Product.Id == id);
                var product = db.Products.FirstOrDefault(x => x.Id == id);
                if (item == null)
                {
                    if (qty > product.Amount)
                    {
                        throw new Exception();
                    }
                    if (people != null)
                    {
                        var cartDetail = new Cart();
                        cartDetail.ProductId = product.Id;
                        cartDetail.UserId = people.Id;
                        cartDetail.Amount = qty;
                        db.Carts.Add(cartDetail);
                        db.SaveChanges();
                    }

                    item = new CartItem();
                    item.Product = product;
                    item.Amount = qty;
                    item.Price = PriceUltis.get(item.Product.Price, item.Product.Discount ?? 0);
                    item.TotalPrice = item.Price * item.Amount;
                    cart.CartItems.Insert(0, item);
                }
                else
                {
                    if (people != null)
                    {
                        var cartDetail = db.Carts.FirstOrDefault(x => x.ProductId == item.Product.Id && x.UserId == people.Id);
                        if (cartDetail.Amount + qty > product.Amount)
                        {
                            throw new Exception();
                        }
                        cartDetail.Amount += qty;

                        db.SaveChanges();
                    }
                    if (item.Amount + qty > product.Amount)
                    {
                        throw new Exception();
                    }
                    item.Amount += qty;
                    item.Price = PriceUltis.get(item.Product.Price, item.Product.Discount ?? 0);
                    item.TotalPrice = item.Price * item.Amount;
                }

                Session["Cart"] = cart;
                return new JsonResult() { Data = "", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception)
            {
                return Json(new { Error = true, Message = "Số lượng mua lớn hơn số lượng hiện có!!!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckQty(int id, int qty)
        {
            try
            {
                var people = Session["info"] as Person;
                CartTemp cart = getCart();
                cart.CartItems.ForEach(x => x.IsChecked = false);
                CartItem item = cart.CartItems.Find(model => model.Product.Id == id);
                var product = db.Products.FirstOrDefault(x => x.Id == id);

                if (qty > product.Amount)
                {
                    throw new Exception();
                }

                return Json(new { Error = false, Message = "Số lượng mua lớn hơn số lượng hiện có!!!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Error = true, Message = "Số lượng mua lớn hơn số lượng hiện có!!!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult BuyNow(int id, int qty) //  Add item in cart 
        {
            try
            {
                var people = Session["info"] as Person;
                CartTemp cart = getCart();
                cart.CartItems.ForEach(x => x.IsChecked = false);
                CartItem item = cart.CartItems.Find(model => model.Product.Id == id);
                var product = db.Products.FirstOrDefault(x => x.Id == id);

                if (item == null)
                {
                    if (qty > product.Amount)
                    {
                        throw new Exception();
                    }
                    item = new CartItem();
                    item.Product = product;
                    item.Amount = qty;
                    item.Price = PriceUltis.get(item.Product.Price, item.Product.Discount ?? 0);
                    item.TotalPrice = item.Price * item.Amount;
                    item.IsChecked = true;
                    cart.CartItems.Add(item);
                }
                else
                {
                    if (qty > product.Amount)
                    {
                        throw new Exception();
                    }
                    item.Amount = qty;
                    item.Price = PriceUltis.get(item.Product.Price, item.Product.Discount ?? 0);
                    item.TotalPrice = item.Price * item.Amount;
                    item.IsChecked = true;
                }

                cart.Total = ((long)cart.CartItems.Where(x => x.IsChecked == true).Sum(x => x.Price * x.Amount));
                return RedirectToAction("Checkout");
            }
            catch (Exception)
            {
                return Json(new {Error=true, Message= "Số lượng mua lớn hơn số lượng hiện có!!!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CheckedAllProduct(bool IsChecked)
        {
            try
            {
                CartTemp cart = getCart();

                cart.CartItems.ForEach(x => x.IsChecked = IsChecked);
                var result = cart.CartItems.Select(e => new
                {
                    Id = e.Product.Id,
                    IsChecked = e.IsChecked,
                    Price = e.Price,
                    Amount = e.Amount,
                    TotalPrice = e.TotalPrice,
                    OldPrice = e.Product.Price,
                });

                cart.Total = ((long)cart.CartItems.Where(x => x.IsChecked == true).Sum(x => x.Price * x.Amount));

                Session["Cart"] = cart;
                return new JsonResult() { Data = new { list = result, total = cart.Total }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult AddCheckedProductToBuy(int Id, bool IsChecked)
        {
            try
            {
                CartTemp cart = getCart();

                var item = cart.CartItems.FirstOrDefault(x => x.Product.Id == Id);
                item.IsChecked = IsChecked;

                var result = cart.CartItems.Select(e => new
                {
                    Id = e.Product.Id,
                    IsChecked = e.IsChecked,
                    Price = e.Price,
                    Amount = e.Amount,
                    TotalPrice = e.TotalPrice,
                    OldPrice = e.Product.Price,
                });

                cart.Total = ((long)cart.CartItems.Where(x => x.IsChecked == true).Sum(x => x.Price * x.Amount));
                Session["Cart"] = cart;
                return new JsonResult() { Data = new { list = result, total = cart.Total }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult SubtractQty(int Id)
        {
            try
            {
                var people = Session["info"] as Person;
                CartTemp cart = getCart();

                var item = cart.CartItems.FirstOrDefault(x => x.Product.Id == Id);
                item.Amount = Math.Max(1, item.Amount - 1);
                item.Price = PriceUltis.get(item.Product.Price, item.Product.Discount ?? 0);
                item.TotalPrice = item.Price * item.Amount;

                if (people != null)
                {
                    var cartDetail = db.Carts.FirstOrDefault(x => x.ProductId == item.Product.Id && x.UserId == people.Id);
                    cartDetail.Amount = item.Amount;
                    db.SaveChanges();
                }

                var result = new
                {
                    Id = item.Product.Id,
                    IsChecked = item.IsChecked,
                    Amount = item.Amount,
                    Price = item.Price,
                    TotalPrice = item.TotalPrice,
                    OldPrice = item.Product.Price,
                };
                var list = cart.CartItems.Select(e => new
                {
                    Id = e.Product.Id,
                    IsChecked = e.IsChecked,
                    Price = e.Price,
                    Amount = e.Amount,
                    TotalPrice = e.TotalPrice,
                    OldPrice = e.Product.Price,
                });
                cart.Total = ((long)cart.CartItems.Where(x => x.IsChecked == true).Sum(x => x.Price * x.Amount));
                Session["Cart"] = cart;

                return new JsonResult() { Data = new { item = result, list = list, total = cart.Total }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult AddQty(int Id)
        {
            try
            {
                var people = Session["info"] as Person;
                CartTemp cart = getCart();

                var item = cart.CartItems.FirstOrDefault(x => x.Product.Id == Id);
                item.Amount = item.Amount + 1;
                item.Price = PriceUltis.get(item.Product.Price, item.Product.Discount ?? 0);
                item.TotalPrice = item.Price * item.Amount;

                if (people != null)
                {
                    var cartDetail = db.Carts.FirstOrDefault(x => x.ProductId == item.Product.Id && x.UserId == people.Id);
                    cartDetail.Amount = item.Amount;
                    db.SaveChanges();
                }

                var result = new
                {
                    Id = item.Product.Id,
                    IsChecked = item.IsChecked,
                    Amount = item.Amount,
                    Price = item.Price,
                    TotalPrice = item.TotalPrice,
                    OldPrice = item.Product.Price,
                };
                var list = cart.CartItems.Select(e => new
                {
                    Id = e.Product.Id,
                    IsChecked = e.IsChecked,
                    Price = e.Price,
                    Amount = e.Amount,
                    TotalPrice = e.TotalPrice,
                    OldPrice = e.Product.Price,
                });
                cart.Total = ((long)cart.CartItems.Where(x => x.IsChecked == true).Sum(x => x.Price * x.Amount));
                Session["Cart"] = cart;

                return new JsonResult() { Data = new { item = result, list = list, total = cart.Total }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult UpdateQty(int Id, int Amount)
        {
            try
            {
                var people = Session["info"] as Person;
                CartTemp cart = getCart();

                var item = cart.CartItems.FirstOrDefault(x => x.Product.Id == Id);
                item.Amount = Math.Max(1, Amount);
                item.Price = PriceUltis.get(item.Product.Price, item.Product.Discount ?? 0);
                item.TotalPrice = item.Price * item.Amount;

                if (people != null)
                {
                    var cartDetail = db.Carts.FirstOrDefault(x => x.ProductId == item.Product.Id && x.UserId == people.Id);
                    cartDetail.Amount = item.Amount;
                    db.SaveChanges();
                }

                var result = new
                {
                    Id = item.Product.Id,
                    IsChecked = item.IsChecked,
                    Amount = item.Amount,
                    Price = item.Price,
                    TotalPrice = item.TotalPrice,
                    OldPrice = item.Product.Price,
                };
                var list = cart.CartItems.Select(e => new
                {
                    Id = e.Product.Id,
                    IsChecked = e.IsChecked,
                    Price = e.Price,
                    Amount = e.Amount,
                    TotalPrice = e.TotalPrice,
                    OldPrice = e.Product.Price,
                });

                cart.Total = ((long)cart.CartItems.Where(x => x.IsChecked == true).Sum(x => x.Price * x.Amount));
                Session["Cart"] = cart;

                return new JsonResult() { Data = new { item = result, list = list, total = cart.Total }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult DeleteItem(int Id)
        {
            try
            {
                var people = Session["info"] as Person;
                CartTemp cart = getCart();

                var item = cart.CartItems.FirstOrDefault(x => x.Product.Id == Id);
                cart.CartItems.Remove(item);

                if (people != null)
                {
                    var cartDetail = db.Carts.FirstOrDefault(x => x.ProductId == item.Product.Id && x.UserId == people.Id);
                    if (cartDetail != null)
                    {
                        db.Carts.Remove(cartDetail);
                        db.SaveChanges();
                    }
                }

                var result = new
                {
                    Id = Id,
                };
                var list = cart.CartItems.Select(e => new
                {
                    Id = e.Product.Id,
                    IsChecked = e.IsChecked,
                    Price = e.Price,
                    Amount = e.Amount,
                    TotalPrice = e.TotalPrice,
                    OldPrice = e.Product.Price,
                });

                cart.Total = ((long)cart.CartItems.Where(x => x.IsChecked == true).Sum(x => x.Price * x.Amount));
                Session["Cart"] = cart;

                return new JsonResult() { Data = new { item = result, list = list, total = cart.Total }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public async Task<ActionResult> Checkout()
        {
            try
            {
                var people = Session["info"] as Person;
                if (people != null)
                {
                    people = db.People.FirstOrDefault(x => x.Id == people.Id);

                    ViewData["info"] = people;

                    var addressUltis = new AddressUltis();
                    if (people.Province != null && people.Province != string.Empty)
                    {
                        var province = await addressUltis.GetProvinceByName(people.Province != string.Empty && people.Province != null ? people.Province : "");
                        var district = await addressUltis.GetDistrictByName(people.District != string.Empty && people.District != null ? people.District : "", province.ProvinceID);
                        var ward = await addressUltis.GetWardByName(people.Ward != string.Empty && people.Ward != null ? people.Ward : "", district.DistrictID);

                        ViewData["province"] = province;
                        ViewData["district"] = district;
                        ViewData["ward"] = ward;
                    }
                    else
                    {
                        ViewData["province"] = null;
                        ViewData["district"] = null;
                        ViewData["ward"] = null;
                    }
                }

                var customers = db.People.Select(ps => new Customer
                {
                    CustomerId = ps.Id,
                    PurchasedProducts = db.Products
                    .Join(db.OrderDetails, p => p.Id, od => od.ProductId, (p, od) => new { Product = p, OrderDetail = od })
                    .Join(db.Orders, x => x.OrderDetail.OrderId, o => o.Id, (x, o) => new { x.Product, Order = o })
                    .Where(x => x.Order.CustomerId == ps.Id)
                    .Select(x => x.Product.Id)
                    .ToList()
                }).ToList();

                CartTemp cart = getCart();
                cart.Discounts.Clear();
                if (people != null)
                {
                    var me = customers.FirstOrDefault(x => x.CustomerId == people.Id);
                    me.PurchasedProducts.Clear();
                    me.PurchasedProducts.AddRange(cart.CartItems.Where(c => c.IsChecked == true).Select(c => c.Product.Id).ToList());

                    var productIdsRecommend = Recommender.GetRecommendedProducts(customers, people.Id);
                    var productsRecommend = db.Products.Where(x => productIdsRecommend.Contains(x.Id)).ToList();
                    ViewData["productsRecommend"] = productsRecommend;
                }
                else
                {
                    var me = new Customer();
                    me.CustomerId = -1000;
                    me.PurchasedProducts.Clear();
                    me.PurchasedProducts.AddRange(cart.CartItems.Where(c => c.IsChecked == true).Select(c => c.Product.Id).ToList());
                    customers.Add(me);

                    var productIdsRecommend = Recommender.GetRecommendedProducts(customers, me.CustomerId);
                    var productsRecommend = db.Products.Where(x => productIdsRecommend.Contains(x.Id)).ToList();
                    ViewData["productsRecommend"] = productsRecommend;
                }

                return View();
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetShipFeeAsync(PostGHNFee feeData)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
                        string data = JsonConvert.SerializeObject(feeData);

                        HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");

                        client.DefaultRequestHeaders.Add("Token", Properties.Setting.Token_API_GHN.ToString());
                        client.DefaultRequestHeaders.Add("ShopId", Properties.Setting.ShopID_API_GHN.ToString());
                        HttpResponseMessage response = await client.PostAsync("https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/fee", content);


                        if (response.IsSuccessStatusCode)
                        {
                            string res = await response.Content.ReadAsStringAsync();
                            return Json(res, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult PlusFeeShip(int fee)
        {
            try
            {
                CartTemp cart = getCart();
                cart.FeeShip = fee;

                PrepareCart prepare = calcCart(cart);
                cart.TotalPaid = prepare.totalPaid;
                cart.SumDiscountValue = prepare.sumDiscountValue;
                cart.Total = prepare.total;

                return new JsonResult()
                {
                    Data = new
                    {
                        totalPaid = cart.TotalPaid,
                        feeShip = cart.FeeShip,
                        total = cart.Total,
                        Discounts = cart.Discounts.Select(x => new { x.Code, x.Title, x.Value, x.Type, x.DateExpired, x.Amount, x.DateStart, x.ApplyField, x.ConditionalOperator, x.ConditionalPrice, x.Content, x.Used, x.SubTitle, x.LimitUsed }).ToList(),
                        SumDiscountValue = cart.SumDiscountValue,
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult GetAllDiscount(List<int> pIds)
        {
            try
            {
                CartTemp cart = getCart();
                var total = (long)cart.CartItems.Where(x => x.IsChecked == true).Sum(x => x.Price * x.Amount);
                List<Discount> list = db.Discounts.ToList().FindAll(x => DiscountUltis.checkDiscount(x, pIds) && DiscountUltis.checkApply(total, x.ConditionalOperator, x.ConditionalPrice)).ToList();

                return Json(list.Select(x => new { x.Code, x.Title, x.Value, x.Type, x.DateExpired, x.Amount, x.DateStart, x.ApplyField, x.ConditionalOperator, x.ConditionalPrice, x.Content, x.Used, x.SubTitle, x.LimitUsed }).ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult ApplyDiscount(string code)
        {
            try
            {
                CartTemp cart = getCart();

                var listPId = cart.CartItems.Select(p => p.Product.Id).ToList();
                var discount = db.Discounts.FirstOrDefault(d => d.Code == code);
                var check = DiscountUltis.checkDiscount(discount, listPId);

                if (check)
                {
                    if (discount.ApplyField == "Price" && cart.Discounts.Count() <= 2)
                    {
                        var discountPrice = cart.Discounts.FirstOrDefault(d => d.ApplyField == "Price");
                        if (discountPrice != null)
                        {
                            cart.Discounts.Remove(discountPrice);
                        }

                        if (DiscountUltis.checkApply(cart.Total, discount.ConditionalOperator, discount.ConditionalPrice))
                            cart.Discounts.Add(discount);
                    }

                    if (discount.ApplyField.Equals("FeeShip") && cart.Discounts.Count() <= 2)
                    {
                        var discountFeeShip = cart.Discounts.FirstOrDefault(d => d.ApplyField == "FeeShip");
                        if (discountFeeShip != null)
                        {
                            cart.Discounts.Remove(discountFeeShip);
                        }
                        if (DiscountUltis.checkApply(cart.Total, discount.ConditionalOperator, discount.ConditionalPrice))
                            cart.Discounts.Add(discount);
                    }

                    PrepareCart prepare = calcCart(cart);
                    cart.TotalPaid = prepare.totalPaid;
                    cart.SumDiscountValue = prepare.sumDiscountValue;
                    cart.Total = prepare.total;

                    return new JsonResult()
                    {
                        Data = new
                        {
                            Discounts = cart.Discounts.Select(x => new { x.Code, x.Title, x.Value, x.Type, x.DateExpired, x.Amount, x.DateStart, x.ApplyField, x.ConditionalOperator, x.ConditionalPrice, x.Content, x.Used, x.SubTitle, x.LimitUsed }).ToList(),
                            totalPaid = cart.TotalPaid,
                            SumDiscountValue = cart.SumDiscountValue,
                            Total = cart.Total,
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Hello");
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult RemoveDiscount(string code)
        {
            try
            {
                CartTemp cart = getCart();
                var discount = cart.Discounts.FirstOrDefault(d => d.Code == code);


                if (discount != null)
                    cart.Discounts.Remove(discount);

                PrepareCart prepare = calcCart(cart);
                cart.TotalPaid = prepare.totalPaid;
                cart.SumDiscountValue = prepare.sumDiscountValue;
                cart.Total = prepare.total;

                return new JsonResult()
                {
                    Data = new
                    {
                        Discounts = cart.Discounts.Select(x => new { x.Code, x.Title, x.Value, x.Type, x.DateExpired, x.Amount, x.DateStart, x.ApplyField, x.ConditionalOperator, x.ConditionalPrice, x.Content, x.Used, x.SubTitle, x.LimitUsed }).ToList(),
                        totalPaid = cart.TotalPaid,
                        SumDiscountValue = cart.SumDiscountValue,
                        Total = cart.Total,
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Hello");
            }
        }

        public PrepareCart calcCart(CartTemp cart)
        {
            var total = (long)cart.CartItems.Where(x => x.IsChecked == true).Sum(x => x.Price * x.Amount);
            var sumDiscountValue = 0l;
            total += cart.FeeShip;

            foreach (var discountItem in cart.Discounts)
            {
                if (discountItem.Type.Equals("VNÐ"))
                {
                    if (discountItem.ApplyField.Equals("Price"))
                    {
                        sumDiscountValue += Math.Min(total, discountItem.Value);
                    }
                    if (discountItem.ApplyField.Equals("FeeShip"))
                    {
                        sumDiscountValue += Math.Min(cart.FeeShip, discountItem.Value);
                    }
                }

                if (discountItem.Type.Equals("%"))
                {
                    if (discountItem.ApplyField.Equals("Price"))
                    {
                        sumDiscountValue += Math.Min((long)(total * discountItem.Value / 100.0), total);
                    }
                    if (discountItem.ApplyField.Equals("FeeShip"))
                    {
                        sumDiscountValue += Math.Min(((long)(cart.FeeShip * discountItem.Value / 100.0)), cart.FeeShip);
                    }
                }

            }

            var totalPaid = total - sumDiscountValue;

            return new PrepareCart()
            {
                total = (long)cart.CartItems.Where(x => x.IsChecked == true).Sum(x => x.Price * x.Amount),
                sumDiscountValue = sumDiscountValue,
                totalPaid = totalPaid,
            };
        }

        [HttpPost]
        public async Task<ActionResult> ConfirmCheckout(int to_district_id, string to_ward_code, string values)
        {
            try
            {
                CheckoutInfo checkoutInfo = new CheckoutInfo();
                JsonConvert.PopulateObject(values, checkoutInfo);
                CartTemp cart = getCart();
                if (Session["info"] == null)
                {
                    Person people = new Person();
                    people.Name = checkoutInfo.Name;
                    people.Email = checkoutInfo.Email;
                    people.Phone = checkoutInfo.Phone;
                    people.Street = checkoutInfo.Street;
                    people.Ward = checkoutInfo.Ward;
                    people.District = checkoutInfo.District;
                    people.Province = checkoutInfo.Province;

                    people = db.People.Add(people);
                    db.SaveChanges();

                    Order order = new Order();
                    order.OrderDate = DateTime.Now;
                    order.CustomerId = people.Id;
                    order.Street = checkoutInfo.Street;
                    order.Ward = checkoutInfo.Ward;
                    order.District = checkoutInfo.District;
                    order.Province = checkoutInfo.Province;
                    order.PaymentMethod = checkoutInfo.PaymentMethod;

                    var cartItemCheckout = cart.CartItems.Where(x => x.IsChecked == true).ToList();

                    foreach (CartItem item in cartItemCheckout)
                    {
                        var orderDetail = new OrderDetail();
                        orderDetail.OrderId = item.OrderId;
                        orderDetail.ProductId = item.Product.Id;
                        orderDetail.Price = item.Price;
                        orderDetail.Amount = item.Amount;
                        orderDetail.TotalPrice = item.Amount * item.Price;
                        var product = db.Products.FirstOrDefault(x => x.Id == item.Product.Id);
                        product.Amount = Math.Max((product.Amount ?? 0) - item.Amount, 0);

                        order.OrderDetails.Add(orderDetail);
                    }
                    foreach (Discount item in cart.Discounts)
                    {
                        var discount = db.Discounts.FirstOrDefault(d => d.Code == item.Code);
                        discount.Used++;
                        order.Discounts.Add(discount);
                    }

                    HttpClient client = new HttpClient();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
                    string data = JsonConvert.SerializeObject(new
                    {
                        to_name = people.Name,
                        to_phone = people.Phone,
                        to_address = checkoutInfo.Street + ", " + checkoutInfo.Ward + ", " + checkoutInfo.District + ", " + checkoutInfo.Province,
                        to_ward_code = to_ward_code,
                        to_district_id = to_district_id,
                        height = 50,
                        length = 20,
                        weight = 200,
                        width = 20,
                        required_note = "CHOTHUHANG",
                        note = "Fahasa gửi hàng",
                        cod_amount = cart.TotalPaid,
                        service_id = 0,
                        service_type_id = 2,
                        payment_type_id = 1,
                        items = cart.CartItems.Where(x => x.IsChecked == true).Select(x => new
                        {
                            name = x.Product.Name,
                            quantity = x.Amount,
                            code = x.ProductId.ToString(),
                            price = ((int)x.Price),
                            length = 12,
                            width = 12,
                            height = 12,
                            weight = 1200,
                        }),
                    });

                    HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Add("Token", Properties.Setting.Token_API_GHN.ToString());
                    client.DefaultRequestHeaders.Add("ShopId", Properties.Setting.ShopID_API_GHN.ToString());
                    HttpResponseMessage response = await client.PostAsync("https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/create", content);


                    if (response.IsSuccessStatusCode)
                    {
                        string res = await response.Content.ReadAsStringAsync();
                        CreateOrderRes createOrderRes = new CreateOrderRes();
                        JsonConvert.PopulateObject(res, createOrderRes);
                        order.ShipCode = createOrderRes.data.order_code;
                        order.FeeShip = ((int)cart.FeeShip);
                        db.Orders.Add(order);
                        order.Status = "Chờ lấy hàng";
                        db.SaveChanges();

                        var listProId = order.OrderDetails.Select(x => x.ProductId).ToList();
                        ViewData["productsOrderDetail"] = db.Products.Where(x => listProId.Contains(x.Id)).ToList();
                        var message = await MailUltis.SendMailGoogleSmtp(checkoutInfo.Email, checkoutInfo.Email, "Fahasa đã nhận được đơn hàng của bạn!", MailUltis.getBodyMailConfirmCheckout(order), Properties.Setting.Gmail, Properties.Setting.Password);
                        return View("~/Views/Order/Done.cshtml", order);
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    Person people = Session["info"] as Person;
                    people = db.People.FirstOrDefault(x => people.Id == x.Id);

                    people.Name = checkoutInfo.Name;
                    people.Email = checkoutInfo.Email;
                    people.Phone = checkoutInfo.Phone;
                    people.Street = checkoutInfo.Street;
                    people.District = checkoutInfo.District;
                    people.Ward = checkoutInfo.Ward;
                    people.Province = checkoutInfo.Province;

                    Order order = new Order();
                    order.OrderDate = DateTime.Now;
                    order.CustomerId = people.Id;
                    order.Street = checkoutInfo.Street;
                    order.Ward = checkoutInfo.Ward;
                    order.District = checkoutInfo.District;
                    order.Province = checkoutInfo.Province;
                    order.PaymentMethod = checkoutInfo.PaymentMethod;

                    var cartItemCheckout = cart.CartItems.Where(x => x.IsChecked == true).ToList();

                    foreach (CartItem item in cartItemCheckout)
                    {
                        var orderDetail = new OrderDetail();
                        orderDetail.OrderId = item.OrderId;
                        orderDetail.ProductId = item.Product.Id;
                        orderDetail.Price = item.Price;
                        orderDetail.Amount = item.Amount;
                        orderDetail.TotalPrice = item.Amount * item.Price;
                        var product = db.Products.FirstOrDefault(x => x.Id == item.Product.Id);
                        product.Amount = Math.Max((product.Amount ?? 0) - item.Amount, 0);

                        order.OrderDetails.Add(orderDetail);
                    }
                    foreach (Discount item in cart.Discounts)
                    {
                        var discount = db.Discounts.FirstOrDefault(d => d.Code == item.Code);
                        discount.Used++;
                        order.Discounts.Add(discount);
                    }

                    HttpClient client = new HttpClient();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
                    string data = JsonConvert.SerializeObject(new
                    {
                        to_name = people.Name,
                        to_phone = people.Phone,
                        to_address = checkoutInfo.Street + ", " + checkoutInfo.Ward + ", " + checkoutInfo.District + ", " + checkoutInfo.Province,
                        to_ward_code = to_ward_code,
                        to_district_id = to_district_id,
                        height = 50,
                        length = 20,
                        weight = 200,
                        width = 20,
                        required_note = "CHOTHUHANG",
                        note = "Fahasa gửi hàng",
                        cod_amount = cart.TotalPaid,
                        service_id = 0,
                        service_type_id = 2,
                        payment_type_id = 1,
                        items = cart.CartItems.Where(x => x.IsChecked == true).Select(x => new
                        {
                            name = x.Product.Name,
                            quantity = x.Amount,
                            code = x.ProductId.ToString(),
                            price = ((int)x.Price),
                            length = 12,
                            width = 12,
                            height = 12,
                            weight = 1200,
                        }),
                    });

                    HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Add("Token", Properties.Setting.Token_API_GHN.ToString());
                    client.DefaultRequestHeaders.Add("ShopId", Properties.Setting.ShopID_API_GHN.ToString());
                    HttpResponseMessage response = await client.PostAsync("https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/create", content);


                    if (response.IsSuccessStatusCode)
                    {
                        string res = await response.Content.ReadAsStringAsync();
                        CreateOrderRes createOrderRes = new CreateOrderRes();
                        JsonConvert.PopulateObject(res, createOrderRes);
                        order.ShipCode = createOrderRes.data.order_code;
                        order.FeeShip = ((int)cart.FeeShip);
                        order = db.Orders.Add(order);
                        order.Status = "Chờ lấy hàng";
                        db.SaveChanges();

                        order = db.Orders.FirstOrDefault(x => x.Id == order.Id);
                        var listProId = order.OrderDetails.Select(x => x.ProductId).ToList();
                        ViewData["productsOrderDetail"] = db.Products.Where(x => listProId.Contains(x.Id)).ToList();
                        var message = await MailUltis.SendMailGoogleSmtp(people.Email, people.Email, "Fahasa đã nhận được đơn hàng của bạn!", MailUltis.getBodyMailConfirmCheckout(order), Properties.Setting.Gmail, Properties.Setting.Password);


                        if (people != null)
                        {
                            cart.CartItems.Where(x => x.IsChecked == true).ToList().ForEach(item =>
                            {
                                var cartDetail = db.Carts.FirstOrDefault(c => c.ProductId == item.Product.Id && c.UserId == people.Id);
                                if (cartDetail != null)
                                {
                                    db.Carts.Remove(cartDetail);
                                    db.SaveChanges();
                                }
                            });
                        }

                        cart.CartItems = cart.CartItems.Where(x => x.IsChecked != true).ToList();
                        cart.Discounts = new List<Discount>();

                        return View("~/Views/Order/Done.cshtml", order);
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception)
            {

                throw ;
            }
        }

        public async Task<ActionResult> Cancel(int orderId)
        {
            try
            {
                var people = Session["info"] as Person;
                var order = db.Orders.FirstOrDefault(x => x.Id == orderId);

                HttpClient client = new HttpClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
                string data = JsonConvert.SerializeObject(new
                {
                    order_codes = new List<string> { order.ShipCode }
                });

                HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Add("Token", Properties.Setting.Token_API_GHN.ToString());
                client.DefaultRequestHeaders.Add("ShopId", Properties.Setting.ShopID_API_GHN.ToString());
                HttpResponseMessage response = await client.PostAsync("https://dev-online-gateway.ghn.vn/shiip/public-api/v2/switch-status/cancel", content);


                if (response.IsSuccessStatusCode)
                {
                    order.Status = "Đã hủy";
                    db.SaveChanges();

                    var message = await MailUltis.SendMailGoogleSmtp(people.Email, people.Email, "Fahasa đã nhận được yêu cầu HỦY đơn hàng của bạn!", MailUltis.getBodyMailCancelOrder(order), Properties.Setting.Gmail, Properties.Setting.Password);

                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Hello");
            }
        }
    }
}


