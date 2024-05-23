using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fahasa.Models
{
    public class CreateOrderRes
    {
        public OrderCodeObject data {get; set;}
    }
    public class OrderCodeObject {

        public string order_code { get; set; }
    }
}