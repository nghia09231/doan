using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fahasa.Ultis
{
    public class PriceUltis
    {
        public static decimal get(long? Price, int? Discount)
        {
            return Decimal.Parse((
                        Math.Floor(Double.Parse(Price.ToString()) * (1 - Double.Parse(Discount.ToString()) / 100) / 100) * 100
                    ).ToString()
                
                );
        }
        public static string getShow(long? Price)
        {
            return Decimal.Parse(Price.ToString()).ToString("N0", new System.Globalization.CultureInfo("vi-VN")) + " đ";
        }
        public static string getShowDiscount(long? Price, int? Discount)
        {
            return Decimal.Parse((Math.Floor(Double.Parse(Price.ToString()) * (1 - Double.Parse(Discount.ToString()) / 100) / 100) * 100).ToString()).ToString("N0", new System.Globalization.CultureInfo("vi-VN")) + " đ";
        }
    }
}