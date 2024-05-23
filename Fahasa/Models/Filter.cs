using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fahasa.Models
{
    public class Filter
    {
        public FilterItems<int> author { get; set; }
        public FilterItems<string> brand { get; set; }
        public FilterItems<string> categories { get; set; }
        public FilterItems<int> cover { get; set; }
        public FilterItems<string> origin { get; set; }
        public FilterItem<FilterPrice> price { get; set; }
        public FilterItems<int> publisher { get; set; }
        public FilterItems<int> supplier { get; set; }
        public FilterItem<int> take { get; set; }
        public FilterItem<int> skip { get; set; }
        public FilterItem<int> page { get; set; }
        public FilterItem<SortItem> sort { get; set; }
    }
}