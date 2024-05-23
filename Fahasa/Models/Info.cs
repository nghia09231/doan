using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fahasa.Models
{
    public class Info
    {
        private string supplier;
        private string author;
        private string publisher;
        private string cover;
        private string brand;
        private string origin;

        public Info(string supplier, string author, string publisher, string cover)
        {
            this.Supplier = supplier;
            this.Author = author;
            this.Publisher = publisher;
            this.Cover = cover;
        }

        public string Cover { get => cover; set => cover = value; }
        public string Supplier { get => supplier; set => supplier = value; }
        public string Author { get => author; set => author = value; }
        public string Publisher { get => publisher; set => publisher = value; }
        public string Brand { get => brand; set => brand = value; }
        public string Origin { get => origin; set => origin = value; }
    }
}