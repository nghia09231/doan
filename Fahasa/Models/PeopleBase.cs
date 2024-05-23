using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace Fahasa.Models
{
    public class PeopleBase
    {
        public string Avatar {get;set;}
        public string Username {get;set;}
        public string Password {get;set;}
        public string Name {get;set;}
        public string Gender {get;set;}
        public string Address {get;set;}
        public string Birth {get;set;}
        public string Phone {get;set;}
        public string Email {get;set;}
        public GroupBase Group { get; set; }
    }
}