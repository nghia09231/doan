using Fahasa.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static Fahasa.FilterConfig;

namespace Fahasa.Areas.Admin.Controllers
{
    [RedirectingAction]
    public class PermissionController : Controller
    {
        private BookManagementEntities db = new BookManagementEntities();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetPermissions(int? GroupId)
        {
            try
            {
                if (GroupId != null)
                {
                    var groupPermission = db.Groups.FirstOrDefault(g => g.Id == GroupId).Permissions;

                    if (groupPermission != null)
                    {
                        var differentPermissions = db.Permissions
                            .AsEnumerable().Where(x => !groupPermission.Any(gp => gp.Id == x.Id))
                            .AsEnumerable().Select(x => new { x.Id, x.Name, x.Function })
                            .ToList();

                        return Json(differentPermissions, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(db.Permissions.Select(x => new { x.Id, x.Name, x.Function }).ToList(), JsonRequestBehavior.AllowGet);
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
                var permission = new Permission();
                JsonConvert.PopulateObject(values, permission);

                if (!TryValidateModel(permission))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                permission = db.Permissions.Add(permission);
                db.SaveChanges();

                return Json(new
                {
                    Id = permission.Id,
                    Name = permission.Name
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
                var permission = db.Permissions.FirstOrDefault(o => o.Id == key);
                if (permission == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);

                JsonConvert.PopulateObject(values, permission);

                if (!TryValidateModel(permission))
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
                var permission = db.Permissions.FirstOrDefault(model => model.Id == key);

                if (permission == null)
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);

                db.Permissions.Remove(permission);
                db.SaveChanges();
                return Json(new { code = 200, msg = "Xóa thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { code = 500, msg = "Xóa thất bại! " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetEmpty()
        {
            return Json(new List<string>(), JsonRequestBehavior.AllowGet);
        }
    }
}