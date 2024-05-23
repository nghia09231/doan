using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security;
using System.Web;
using System.Web.Mvc;
using Fahasa.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using static Fahasa.FilterConfig;

namespace Fahasa.Areas.Admin.Controllers
{
    [RedirectingAction]
    public class PeopleController : Controller
    {
        private BookManagementEntities db = new BookManagementEntities();

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

        public ActionResult GetPeoples()
        {
            try
            {
                return Json(db.People.Where(x => x.Groups.Count > 0).Select(x => new { x.Id, x.Username, x.Name, x.Gender, x.Street, x.Ward, x.District, x.Province, Birth=x.Birth.ToString(), x.Phone, x.Email, x.Avatar, Lock = x.Lock.ToString(), Groups = x.Groups.Select(g => new { g.Id, g.Name }) }).ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult Add(string values)
        {
            try
            {
                var person = new Person();

                JsonConvert.PopulateObject(values, person);
                var groups = new List<Group>();
                person.Password = Encryptor.MD5Hash(person.Password);

                person.Groups.ToList().ForEach(c =>
                {
                    var category = db.Groups.FirstOrDefault(c2 => c2.Id == c.Id);
                    if (category != null) groups.Add(category);
                });
                if(person.Avatar == null)
                    person.Avatar = "/Content/avatar.jfif";

                person.Groups = groups;

                db.People.Add(person);
                db.SaveChanges();

                return Json(new
                {
                    Id = person.Id,
                    Name = person.Username
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
                var person = db.People.First(p => p.Id == key);

                JsonConvert.PopulateObject(values, person);
                var groups = new List<Group>();

                person.Groups.ToList().ForEach(c =>
                {
                    var group = db.Groups.FirstOrDefault(c2 => c2.Id == c.Id);
                    if (group != null) groups.Add(group);
                });

                person.Groups = groups;

                db.SaveChanges();

                return Json(new
                {
                    Id = person.Id,
                    Name = person.Username
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
                var cover = db.People.FirstOrDefault(model => model.Id == key);

                if (cover == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);

                db.People.Remove(cover);
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
