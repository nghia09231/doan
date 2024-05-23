using Fahasa.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Fahasa.Areas.Admin.Controllers
{
    public class CustomerStatisticsController : Controller
    {
        BookManagementEntities db = new BookManagementEntities();
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
        public string Get(string startDate, string endDate, string type)
        {
            try
            {
                string typeSql = "";
                string groupBySql = "";

                switch (type)
                {
                    case "Ngày":
                        {
                            typeSql = "CONVERT(varchar(10), OrderDate, 103) AS 'Date',";
                            groupBySql = " CONVERT(varchar(10), OrderDate, 103) ";
                            break;
                        }
                    case "Tháng":
                        {
                            typeSql = "CAST(DATEPART (MONTH, [Order].OrderDate) AS varchar(10)) AS 'Date',";
                            groupBySql = "DATEPART (MONTH, [Order].OrderDate)";
                            break;
                        }
                    case "Năm":
                        {
                            typeSql = "CAST(DATEPART (YEAR, [Order].OrderDate) AS varchar(10)) AS 'Date',";
                            groupBySql = "DATEPART (YEAR, [Order].OrderDate)";
                            break;
                        }
                    default:
                        break;
                }


                var revenue = db.Database.SqlQuery<StatisticResult>(@"
                SELECT " + typeSql + @"
                    COUNT(DISTINCT CustomerId) AS ValueInt,
                    SUM(
                    CASE
                        WHEN Discount.Type = 'VNĐ' THEN OrderDetail.TotalPrice - Discount.Value
                        WHEN Discount.Type = '%' THEN OrderDetail.TotalPrice - (OrderDetail.TotalPrice * Discount.Value / 100)
                        ELSE OrderDetail.TotalPrice
                    END
                    ) AS ValueDecimal
                FROM
                    [Order]
                INNER JOIN OrderDetail ON [Order].Id = OrderDetail.OrderId
                LEFT JOIN DiscountOrder ON [Order].Id = DiscountOrder.OrderId
                LEFT JOIN Discount ON DiscountOrder.DiscountCode = Discount.Code
                WHERE
                    [Order].Status = N'Giao hàng thành công' AND ([Order].OrderDate BETWEEN '" + startDate + @"' AND '" + endDate + @"') " + @"
                GROUP BY
                    " + groupBySql + @"
                ORDER BY
                    'Date'").Select(x => x).ToList();

                return JsonConvert.SerializeObject(revenue.Select(x => new { Date = x.Date, x.ValueInt, x.ValueDecimal }));
            }
            catch (Exception)
            {
                Response.StatusCode = 400;
                return JsonConvert.SerializeObject(null);
            }
        }
    }
}