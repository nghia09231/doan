using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fahasa.Models
{
    public class CartTemp
    {
        public List<CartItem> CartItems { get; set; }
        public List<Discount> Discounts { get; set; }
        public long FeeShip { get; set; }
        public long TotalPaid { get; set; }
        public long SumDiscountValue { get; set; }
        public long Total { get; set; }

        public CartTemp() {
            CartItems  = new List<CartItem>();
            Discounts = new List<Discount>();
            FeeShip = 0;
            TotalPaid = 0;
            SumDiscountValue = 0;
            Total = 0;
        }
    }
}