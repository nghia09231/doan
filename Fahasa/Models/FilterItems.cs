using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fahasa.Models
{
    public class FilterItems<T>
    {
        public string type { get; set; }
        public List<T> value { get; set; }
    }
}