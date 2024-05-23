using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Fahasa.Models;
using static Fahasa.FilterConfig;

namespace Fahasa.Controllers
{
    [RefreshAction]
    public class UserController : Controller
    {
        BookManagementEntities db = new BookManagementEntities();

        [HttpPost]
        public ActionResult Login(string Username, string Password)
        {
            try
            {
                Password = Encryptor.MD5Hash(Password);

                var check = db.People.SingleOrDefault(model => model.Username == Username && model.Password == Password);

                if (ModelState.IsValid)
                {
                    if (check == null)
                    {
                        Response.StatusCode = 406;
                        return Json("Đã xảy ra sự cố khi đăng nhập. Hãy kiểm tra tên tài khoản và mật khẩu!!!");
                    }
                    else
                    {
                        if (check.Lock == true)
                        {
                            Response.StatusCode = 403;
                            return Json("Tài khoản đã bị khóa!!!");
                        }

                        Response.StatusCode = 200;
                        DateTime currentTime = DateTime.Now;
                        long timestamp = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
                        var token = JwtService.GenerateToken(new Dictionary<string, object>()
                    {
                        { "Name", check.Name },
                        { "Username", check.Username },
                        { "Avatar", check.Avatar },
                        { "Street", check.Street },
                        { "Ward", check.Ward },
                        { "District", check.District },
                        { "Province", check.Province },
                        { "Email", check.Email },
                        { "Gender", check.Gender },
                        { "Phone", check.Phone },
                        { "exp", timestamp + 600 }
                    });
                        var x = new HttpCookie("fahasaToken", token);
                        Response.Cookies.Add(x);
                        FormsAuthentication.SetAuthCookie(check.Username, true);

                        Session["info"] = check;
                        Session["Cart"] = null;
                        return Json("Đăng nhập thành công!!!");
                    }
                }
                Response.StatusCode = 500;
                return Json("Đăng nhập không thành công!!!");
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult Register(Person member)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var check = db.People.Where(model => model.Username == member.Username).FirstOrDefault();

                    if (check != null)
                    {
                        Response.StatusCode = 406;
                        return Json("Tên tài khoản đã tồn tại");
                    }
                    else
                    {
                        member.Password = Encryptor.MD5Hash(member.Password);
                        member.Avatar = "/Content/avatar.jfif";

                        db.People.Add(member);
                        var result = db.SaveChanges();
                        if (result > 0)
                        {
                            Response.StatusCode = 200;
                            return Json("Tạo tài khoản thành công!!!");
                        }
                    }
                }
                Response.StatusCode = 406;
                return Json("Tạo tài khoản không thành công!!!");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 406;
                return Json("Tạo tài khoản thất bại!!! " + ex.Message);
            }
        }

        public ActionResult Logout()
        {
            var x = new HttpCookie("fahasaToken", "");
            Response.Cookies.Set(x);
            Session.Remove("info");
            Session.Clear();
            Session["info"] = null;
            Session["Cart"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

    }
}