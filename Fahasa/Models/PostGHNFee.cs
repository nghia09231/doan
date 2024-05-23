using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fahasa.Models
{
    public class PostGHNFee
    {
        public string to_ward_code { get; set; }
        public int to_district_id {get;set;}
        public int height {get;set;}
        public int length {get;set;}
        public int weight {get;set;}
        public int width {get;set;}
        public int service_id {get;set;}
    }
}