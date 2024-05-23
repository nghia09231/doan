using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fahasa.Models
{
    public class ProductBase
    {
        public string ImageSrc { get; set; }
        public int Id{ get; set; }
        public string Name { get; set; }
        public long Price { get; set; }
        public string SoonRelease { get; set; }
        public string StockAvailable { get; set; }
        public int? Supplier { get; set; }
        public int? Author { get; set; }
        public int? Cover { get; set; }
        public string Desc { get; set; }
        public string Table { get; set; }
        public int? Amount { get; set; }
        public int? Discount { get; set; }
        public int? Publisher { get; set; }
        public string Brand { get; set; }
        public string Origin { get; set; }
        public List<int> Categories { get; set; }
    }
}