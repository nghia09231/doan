using Fahasa.Models;
using Jose;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.EnterpriseServices.CompensatingResourceManager;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace Fahasa
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            
        }
        public class RedirectingActionAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                base.OnActionExecuting(filterContext);
                var fahasaToken = filterContext.HttpContext.Request.Cookies.Get("fahasaToken");
                var Session = filterContext.HttpContext.Session;

                if (fahasaToken == null)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Login",
                        action = "Index"
                    }));
                }
                else {

                    try
                    {
                        var token = new Token();
                        var decodeTokenJson = JwtService.Decode(fahasaToken.Value);
                        JsonConvert.PopulateObject(decodeTokenJson, token);

                        DateTime currentTime = DateTime.Now;
                        long timestamp = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
                        BookManagementEntities db = new BookManagementEntities();
                        var person = db.People.FirstOrDefault(x => x.Username == token.Username);
                        if (token.exp < timestamp) {
                            if (person != null)
                            {
                                string newToken = JwtService.GenerateToken(new Dictionary<string, object>()
                                {
                                    { "Name", person.Name },
                                    { "Username", person.Username },
                                    { "Avatar", person.Avatar },
                                    { "Street", person.Street },
                                    { "Ward", person.Ward },
                                    { "District", person.District },
                                    { "Province", person.Province },
                                    { "Email", person.Email },
                                    { "Gender", person.Gender },
                                    { "Phone", person.Phone },
                                    { "exp", timestamp + 600 }
                                });

                                var x = new HttpCookie("fahasaToken", newToken);
                                filterContext.HttpContext.Response.Cookies.Add(x);
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }

                        FormsAuthentication.SetAuthCookie(person.Username, true);
                        Session["userNameAdmin"] = person.Username;
                        Session["infoAdmin"] = person;
                        Session["adminGroup"] = person.Groups;
                    }
                    catch (Exception)
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                        {
                            controller = "Login",
                            action = "Index"
                        }));
                    }
                }
            }
        }

        public class RefreshActionAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                base.OnActionExecuting(filterContext);
                var fahasaToken = filterContext.HttpContext.Request.Cookies.Get("fahasaToken");
                var Session = filterContext.HttpContext.Session;

                if (fahasaToken == null)
                {
                    var x = new HttpCookie("fahasaToken", "");
                    filterContext.HttpContext.Response.Cookies.Set(x);
                    Session.Remove("info");
                    Session.Clear();
                    Session["info"] = null;
                    FormsAuthentication.SignOut();
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Home",
                        action = "Index"
                    }));
                    return;
                }

                try
                {
                    var token = new Token();
                    var decodeTokenJson = JwtService.Decode(fahasaToken.Value);
                    JsonConvert.PopulateObject(decodeTokenJson, token);

                    DateTime currentTime = DateTime.Now;
                    long timestamp = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
                    BookManagementEntities db = new BookManagementEntities();
                    var person = db.People.FirstOrDefault(x => x.Username == token.Username);
                    if (token.exp < timestamp)
                    {
                        if (person != null)
                        {
                            string newToken = JwtService.GenerateToken(new Dictionary<string, object>()
                                {
                                    { "Name", person.Name },
                                    { "Username", person.Username },
                                    { "Avatar", person.Avatar },
                                    { "Street", person.Street },
                                    { "Ward", person.Ward },
                                    { "District", person.District },
                                    { "Province", person.Province },
                                    { "Email", person.Email },
                                    { "Gender", person.Gender },
                                    { "Phone", person.Phone },
                                    { "exp", timestamp + 600 }
                                });

                            var x = new HttpCookie("fahasaToken", newToken);
                            filterContext.HttpContext.Response.Cookies.Add(x);
                        }
                        else
                        {
                        }
                    }

                    FormsAuthentication.SetAuthCookie(person.Username, true);
                    Session["info"] = person;
                    Session["userNameAdmin"] = person.Username;
                    Session["infoAdmin"] = person;
                    Session["adminGroup"] = person.Groups;
                }
                catch (Exception)
                {
                }
            }
        }

        public class LoggedActionAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                base.OnActionExecuting(filterContext);
                var fahasaToken = filterContext.HttpContext.Request.Cookies.Get("fahasaToken");
                var Session = filterContext.HttpContext.Session;

                if (fahasaToken == null || Session["info"] == null)
                {
                    var x = new HttpCookie("fahasaToken", "");
                    filterContext.HttpContext.Response.Cookies.Set(x);
                    Session.Remove("info");
                    Session.Clear();
                    Session["info"] = null;
                    FormsAuthentication.SignOut();
                     filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Home",
                        action = "Index"
                    }));
                }
            }
        }
    }
}
