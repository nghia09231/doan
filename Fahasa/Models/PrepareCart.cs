using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fahasa.Models
{
    public class PrepareCart
    {
        public long total {get;set;}
        public long sumDiscountValue { get;set;}
        public long totalPaid { get;set; }
    }
}