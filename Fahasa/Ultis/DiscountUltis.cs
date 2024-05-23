using Fahasa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace Fahasa.Ultis
{
    public class DiscountUltis
    {
        public static bool checkDiscount(Discount item) {
            if ((item.Amount == null && item.DateExpired == null) ||
                (item.Amount == null && item.DateExpired != null && DateTime.Now.CompareTo(item.DateExpired) < 0) ||
                (item.Amount != null && item.DateExpired == null && item.Amount > item.Used) ||
                (item.Amount != null && item.DateExpired != null && item.Amount > item.Used && DateTime.Now.CompareTo(item.DateExpired) < 0))
            {
                return true;
            }
            return false;
        }

        public static bool checkDiscount(Discount item, int pId)
        {
            if ((item.Amount == null && item.DateExpired == null) ||
                (item.Amount == null && item.DateExpired != null && DateTime.Now.CompareTo(item.DateExpired) < 0) ||
                (item.Amount != null && item.DateExpired == null && item.Amount > item.Used) ||
                (item.Amount != null && item.DateExpired != null && item.Amount > item.Used && DateTime.Now.CompareTo(item.DateExpired) < 0) &&
                ((item.Products.Count > 0 && item.Products.Any(p => p.Id == pId)) || item.Products.Count == 0)
            )
            {
                return true;
            }
            return false;
        }

        public static bool checkDiscount(Discount item, List<int> pIds)
        {
            if ((item.Amount == null && item.DateExpired == null) ||
                (item.Amount == null && item.DateExpired != null && DateTime.Now.CompareTo(item.DateExpired) < 0) ||
                (item.Amount != null && item.DateExpired == null && item.Amount > item.Used) ||
                (item.Amount != null && item.DateExpired != null && item.Amount > item.Used && DateTime.Now.CompareTo(item.DateExpired) < 0) &&
                ((item.Products.Count > 0 && item.Products.Any(p => pIds.Contains(p.Id))) || item.Products.Count == 0)
            )
            {
                return true;
            }
            return false;
        }

        public static bool checkDiscount(Discount item, int pId, int uId)
        {
            if ((item.Amount == null && item.DateExpired == null) ||
                (item.Amount == null && item.DateExpired != null && DateTime.Now.CompareTo(item.DateExpired) < 0) ||
                (item.Amount != null && item.DateExpired == null && item.Amount > item.Used) ||
                (item.Amount != null && item.DateExpired != null && item.Amount > item.Used && DateTime.Now.CompareTo(item.DateExpired) < 0) &&
                ((item.Products.Count > 0 && item.Products.Any(p => p.Id == pId)) || item.Products.Count == 0) &&
                (item.Orders.ToList().FindAll(x => x.CustomerId == uId).Count == 0)
            )
            {
                return true;
            }
            return false;
        }

        public static bool checkDiscount(Discount item, List<int> pIds, int uId)
        {
            if ((item.Amount == null && item.DateExpired == null) ||
                (item.Amount == null && item.DateExpired != null && DateTime.Now.CompareTo(item.DateExpired) < 0) ||
                (item.Amount != null && item.DateExpired == null && item.Amount > item.Used) ||
                (item.Amount != null && item.DateExpired != null && item.Amount > item.Used && DateTime.Now.CompareTo(item.DateExpired) < 0) &&
                ((item.Products.Count > 0 && item.Products.Any(p => pIds.Contains(p.Id))) || item.Products.Count == 0) &&
                (item.Orders.ToList().FindAll(x => x.CustomerId == uId).Count == 0)
            )
            {
                return true;
            }
            return false;
        }

        public static bool checkApply(long price, string conditionalOperator, long value)
        {
            switch (conditionalOperator.Trim()) {
                case ">": 
                    return price > value;
                case "<":
                    return price < value;
                case ">=":
                    return price >= value;
                case "<=": 
                    return price <= value;
            }

            return false;
        }
    }
}