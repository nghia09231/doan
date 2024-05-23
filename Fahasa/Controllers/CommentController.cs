using Fahasa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fahasa.Controllers
{
    public class CommentController : Controller
    {
        BookManagementEntities db = new BookManagementEntities();

        // GET: Comment
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Create(Comment comment)
        {
            try
            {
                var people = Session["info"] as Person;
                comment.UserId = people.Id;
                comment.Date = DateTime.Now;

                db.Comments.Add(comment);
                db.SaveChanges();
                return new JsonResult { Data = "success" };
            }
            catch (Exception)
            {
                Response.StatusCode = 502;
                return new JsonResult { Data = "error" };
            }

        }
    }
}