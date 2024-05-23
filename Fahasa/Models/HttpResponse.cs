using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fahasa.Models
{
    public class HttpResponse<T>
    {
        public int code {get;set;}
        public string message {get;set;}
        public T data { get; set; }
    }
}