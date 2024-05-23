using Fahasa.Models;
using Fahasa.Ultis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using static Fahasa.FilterConfig;

namespace Fahasa.Controllers
{
    [RefreshAction]
    [LoggedAction]
    public class CustomerController : Controller
    {
        BookManagementEntities db = new BookManagementEntities();
        public ActionResult Index()
        {
            try
            {
                var people = Session["info"] as Person;
                ViewData["info"] = db.People.First(x => x.Id == people.Id);
                return View();
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult UpdateInfo(string values)
        {
            try
            {
                var infoUpdate = new ChangeInfoData();
                var people = Session["info"] as Person;
                people = db.People.First(x => x.Id == people.Id);
                string password = people.Password;

                JsonConvert.PopulateObject(values, infoUpdate);
                if (infoUpdate.CurrentPassword != string.Empty && infoUpdate.CurrentPassword != null && Encryptor.MD5Hash(infoUpdate.CurrentPassword) == people.Password && infoUpdate.Password.Length >= 6)
                {
                    JsonConvert.PopulateObject(values, people);
                    people.Password = Encryptor.MD5Hash(infoUpdate.Password);
                    db.SaveChanges();
                    return Json("Thanh cong1", JsonRequestBehavior.AllowGet);
                }

                if (infoUpdate.CurrentPassword == string.Empty || infoUpdate.CurrentPassword == null)
                {
                    JsonConvert.PopulateObject(values, people);
                    people.Password = password;
                    db.SaveChanges();
                    return Json("Thanh cong2", JsonRequestBehavior.AllowGet);
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult OrderHistory(string type = "Tất cả")
        {
            try
            {
                var people = Session["info"] as Person;
                people = db.People.First(x => x.Id == people.Id);
                if (people == null)
                {
                    ViewData["personOrder"] = new List<Order>();
                }
                else
                {
                    var orders = new List<Order>();

                    if (type != "Tất cả")
                    {
                        orders = db.Orders.Where(x => x.CustomerId == people.Id && x.Status == type).OrderByDescending(x => x.OrderDate).Take(8).ToList();
                    }
                    else
                    {
                        orders = db.Orders.Where(x => x.CustomerId == people.Id).OrderByDescending(x => x.OrderDate).Take(8).ToList();
                    }

                    ViewData["personOrder"] = orders;
                }
                return View();
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
    }
}