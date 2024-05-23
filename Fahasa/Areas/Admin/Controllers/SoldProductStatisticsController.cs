using Fahasa.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Fahasa.Areas.Admin.Controllers
{
    public class SoldProductStatisticsController : Controller
    {
        BookManagementEntities db = new BookManagementEntities();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string Get(string startDate, string endDate, int top = 10)
        {
            try
            {
                var revenue = db.Database.SqlQuery<StatisticResult>(@"
                SELECT TOP " + top.ToString() + @"
                    p.Name AS Title,
                    SUM(od.Amount) AS ValueInt
                FROM
                    [Order] AS o
                JOIN
                    OrderDetail AS od ON o.Id = od.OrderId
                JOIN
                    Product AS p ON od.ProductId = p.Id
                WHERE
                  o.Status = N'Giao hàng thành công' AND (o.OrderDate BETWEEN '" + startDate + @"' AND '" + endDate + @"') " + @"
                GROUP BY
                    p.Id, p.Name
                ORDER BY
                    ValueInt DESC").Select(x => x).ToList();

            return JsonConvert.SerializeObject(revenue.Select(x => new { x.Title, x.ValueInt }));
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return JsonConvert.SerializeObject(null);
            }
        }
    }
}
