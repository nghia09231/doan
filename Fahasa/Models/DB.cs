using Fahasa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fahasa.Controllers
{
    public class DB
    {
        private string id;
        private string image_src;
        private string name_a_label;
        private string price;
        private string product_id;
        private int discount_percent;
        private string type_id;
        private int soon_release;
        private string stock_available;
        private Info info;
        private string desc;
        private string table;
        private List<string> images;
        private List<string> categories;

        public string Id { get => id; set => id = value; }
        public string Image_src { get => image_src; set => image_src = value; }
        public string Name_a_label { get => name_a_label; set => name_a_label = value; }
        public string Price { get => price; set => price = value; }
        public string Product_id { get => product_id; set => product_id = value; }
        public int Discount_percent { get => discount_percent; set => discount_percent = value; }
        public string Type_id { get => type_id; set => type_id = value; }
        public int Soon_release { get => soon_release; set => soon_release = value; }
        public string Stock_available { get => stock_available; set => stock_available = value; }
        public Info Info { get => info; set => info = value; }
        public string Desc { get => desc; set => desc = value; }
        public string Table { get => table; set => table = value; }
        public List<string> Images { get => images; set => images = value; }
        public List<string> Categories { get => categories; set => categories = value; }

        public DB(string id, string image_src, string name_a_label, string price, string product_id, int discount_percent, string type_id, int soon_release, string stock_available, Info info, string desc, string table, List<string> images)
        {
            this.Id = id;
            this.Image_src = image_src;
            this.Name_a_label = name_a_label;
            this.Price = price;
            this.Product_id = product_id;
            this.Discount_percent = discount_percent;
            this.Type_id = type_id;
            this.Soon_release = soon_release;
            this.Stock_available = stock_available;
            this.Info = info;
            this.Desc = desc;
            this.Table = table;
            this.Images = images;
        }


    }
}