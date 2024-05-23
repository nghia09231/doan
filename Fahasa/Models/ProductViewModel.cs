using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fahasa.Models
{
    public class ProductViewModel : Product
    {
        public new int Id { get; set; }
        public new string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public new long Price { get; set; }
        public new string Brand { get; set; }
        public new string Origin { get; set; }
        public new string ImageSrc { get; set; }
        public new int? Discount { get; set; }

        public double? Rating { get; set; }
        public int? RatingCount { get; set; }
    }
}