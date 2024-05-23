using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fahasa.Models;
using System.Web.Security;
using System.Text.RegularExpressions;
using Jose;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Fahasa.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        BookManagementEntities db = new BookManagementEntities();

        [HttpGet]
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

        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            try
            {
                var tk = collection["username"];
                var mk = collection["password"];
                mk = Encryptor.MD5Hash(mk);

                var notMember = db.People.Where(model => model.Groups.Count == 0).FirstOrDefault(model => model.Username == tk && model.Password == mk);
                var check = db.People.Where(model => model.Groups.Count != 0).FirstOrDefault(model => model.Username == tk && model.Password == mk);
                if (ModelState.IsValid)
                {
                    if (check != null && check.Lock != null && check.Lock == true)
                    {
                        ModelState.AddModelError("", "Tài khoản đã bị khóa!!!");
                        return View();
                    }

                    if (check == null)
                    {
                        if (notMember != null)
                        {
                            ModelState.AddModelError("", "Không có quyền truy cập!!!");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Đã xảy ra lỗi. Vui lòng kiểm tra tài khoản và mật khẩu!!!");
                        }
                    }

                    else
                    {
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
                        Session["userNameAdmin"] = check.Username;
                        Session["infoAdmin"] = check;
                        return RedirectToAction("Index", "DashBoard");
                    }
                }
                return View();
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }



        [HttpPost]
        public ActionResult Verify(FormCollection collection)
        {
            try
            {
                var token = collection["token"];

                return Json(JwtService.Decode(token), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json("Token không hợp lệ!!!", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Logout()
        {
            var x = new HttpCookie("fahasaToken", "");
            Response.Cookies.Set(x);
            Session.Remove("info");
            Session["info"] = null;
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
    }
}