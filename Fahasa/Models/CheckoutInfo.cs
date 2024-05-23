﻿using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fahasa.Models
{
    public class CheckoutInfo
    {
        public string Name {get;set;}
        public string Email {get;set;}
        public string Phone {get;set;}
        public string Province {get;set;}
        public string District {get;set;}
        public string Ward {get;set;}
        public string Street {get;set;}
        public string FeeShip {get;set;}
        public string PaymentMethod {get;set;}
        public string coupon { get; set; }
    }
}