using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fahasa.Models
{
    public class FilterPrice
    {
        public long to {get; set;} 
        public long from {get; set; }
        public string text { get; set; }
    }
}