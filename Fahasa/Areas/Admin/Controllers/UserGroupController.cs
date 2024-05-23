using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Fahasa.Models;
using Newtonsoft.Json;
using static Fahasa.FilterConfig;

namespace Fahasa.Areas.Admin.Controllers
{
    [RedirectingAction]
    public class UserGroupController : Controller
    {
        private BookManagementEntities db = new BookManagementEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetUserGroups()
        {
            return Json(db.Groups.Select(x => new { x.Id, x.Name, Permissions = x.Permissions.Select(r => new { r.Id, r.Name, r.Function })}).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserGroupsWithoutPermission()
        {
            return Json(db.Groups.Select(x => new { x.Id, x.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddPermission(int Id, Permission PermissionAdd)
        {
            try
            {
                var group = db.Groups.FirstOrDefault(g => g.Id == Id);
                PermissionAdd = db.Permissions.FirstOrDefault(p => p.Id == PermissionAdd.Id);

                if (group == null || PermissionAdd == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                group.Permissions.Add(PermissionAdd);
                db.SaveChanges();
                return Json(new
                {
                    Id = group.Id,
                    Name = group.Name
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult Add(string values)
        {
            try
            {
                var group = new Group();
                JsonConvert.PopulateObject(values, group);

                if (!TryValidateModel(group))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                group = db.Groups.Add(group);
                db.SaveChanges();

                return Json(new
                {
                    Id = group.Id,
                    Name = group.Name
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPut]
        public ActionResult Update(int key, string values)
        {
            try
            {
                var group = db.Groups.FirstOrDefault(o => o.Id == key);
                if (group == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);

                JsonConvert.PopulateObject(values, group);

                if (!TryValidateModel(group))
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
        public ActionResult Delete(int key)
        {
            try
            {
                var group = db.Groups.FirstOrDefault(model => model.Id == key);

                if (group == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);

                db.Groups.Remove(group);
                db.SaveChanges();
                return Json(new { code = 200, msg = "Xóa thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { code = 500, msg = "Xóa thất bại! " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete]
        public ActionResult DeletePermission(int GroupId, int PermissionId)
        {
            try
            {
                var group = db.Groups.FirstOrDefault(g => g.Id == GroupId);


                if (group == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);

                group.Permissions.Remove(db.Permissions.FirstOrDefault(p => p.Id == PermissionId));

                db.SaveChanges();
                return Json(new { code = 200, msg = "Xóa thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Xóa thất bại! " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
