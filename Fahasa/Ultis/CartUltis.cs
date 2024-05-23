using Fahasa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fahasa.Ultis
{
    public class CartUltis
    {
        public static PrepareCart calcOrder(long total, List<Discount> discounts, int feeShip)
        {
            var staticTotal = total;
            var sumDiscountValue = 0l;
            total += feeShip;

            if (discounts != null)
            {
                foreach (var discountItem in discounts)
                {
                    if (discountItem.Type.Equals("VNÐ"))
                    {
                        if (discountItem.ApplyField.Equals("Price"))
                        {
                            sumDiscountValue += Math.Min(total, discountItem.Value);
                        }
                        if (discountItem.ApplyField.Equals("FeeShip"))
                        {
                            sumDiscountValue += Math.Min(feeShip, discountItem.Value);
                        }
                    }

                    if (discountItem.Type.Equals("%"))
                    {
                        if (discountItem.ApplyField.Equals("Price"))
                        {
                            sumDiscountValue += Math.Min((long)(total * discountItem.Value / 100.0), total);
                        }
                        if (discountItem.ApplyField.Equals("FeeShip"))
                        {
                            sumDiscountValue += Math.Min(((long)(feeShip * discountItem.Value / 100.0)), feeShip);
                        }
                    }
                }
            }

            var totalPaid = total - sumDiscountValue;

            return new PrepareCart()
            {
                total = staticTotal,
                sumDiscountValue = sumDiscountValue,
                totalPaid = totalPaid,
            };
        }
    }
}