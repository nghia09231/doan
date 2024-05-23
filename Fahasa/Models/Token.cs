using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fahasa.Models
{
    public class Token
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public long exp { get; set; }
    }
}