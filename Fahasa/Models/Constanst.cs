using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fahasa.Models
{
    public class Constanst
    {
        public static List<string> DISCOUNT_TYPES = new List<string> { "VNĐ", "%" };
        public static List<string> DISCOUNT_FIELDS = new List<string> { "Price", "FeeShip" };

        public static List<object> COMPARE_OPERATOR = new List<object>
        {
            new {key = ">", text = "Lớn hơn" },
            new {key = ">=", text = "Lớn hơn hoặc bằng" },
            new {key = "<", text = "Nhỏ hơn" },
            new {key = "<=", text = "Nhỏ hơn hoặc bằng " },
        };

        public static List<int> SEARCH_TAKES = new List<int> { 12, 24, 48 };
        public static float SEARCH_TAKE_DEFAULT = 48f;
        public static int SEARCH_MAX_DISTANCE = 2;
    }
}