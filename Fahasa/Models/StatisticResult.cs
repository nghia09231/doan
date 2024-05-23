using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Fahasa.Models
{
    public class StatisticResult
    {
        public string Date { get; set; }
        public decimal ValueDecimal { get; set; }
        public int ValueInt { get; set; }
        public string Title { get; set; }
    }
}