using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Fahasa.Models;
using Fahasa.Ultis;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using static Fahasa.FilterConfig;

namespace Fahasa.Areas.Admin.Controllers
{
    [RedirectingAction]
    public class BuyerController : Controller
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

        public ActionResult GetCustomers(string filter, int skip = 0, int take = 20)
        {
            //try
            //{
            //    return Json(db.People.Where(x => x.Groups.Count == 0).ToList(), JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}

            try
            {
                IQueryable<Person> store = db.People.AsQueryable();

                if (filter != null)
                {
                    var filterObjects = Fahasa.Ultis.Filter.Convert(filter);

                    var parameter = System.Linq.Expressions.Expression.Parameter(typeof(Person), "p");
                    Expression<Func<Person, bool>> expr = null;

                    for (int i = 0; i < filterObjects.Count; i += 2)
                    {
                        var condition = ExpressionFilter.Build<Person>(parameter, filterObjects[i]);
                        if (expr == null)
                            expr = condition;
                        else if ((string)filterObjects[i - 1] == "or")
                            expr = System.Linq.Expressions.Expression.Lambda<Func<Person, bool>>(System.Linq.Expressions.Expression.Or(expr.Body, condition.Body), parameter);
                        else
                            expr = System.Linq.Expressions.Expression.Lambda<Func<Person, bool>>(System.Linq.Expressions.Expression.And(expr.Body, condition.Body), parameter);
                    }

                    store = db.People.Where(expr);
                }

                var customers = store
                    .OrderByDescending(model => model.Id)
                    .Select(x => new
                    {
                        x.Id,
                        x.Username,
                        x.Name,
                        x.Gender,
                        x.Street,
                        x.Ward,
                        x.District,
                        x.Province,
                        Birth = x.Birth.ToString(),
                        x.Phone,
                        x.Email,
                        x.Avatar,
                        x.Lock,
                        Groups = x.Groups.Select(g => new { g.Id, g.Name })
                    });

                var count = customers.Count();

                var result = Json(new { data = customers.Skip(skip).Take(take), totalCount = count }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet)
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
