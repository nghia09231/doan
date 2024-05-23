using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fahasa.Models
{
    public class CartItem:OrderDetail
    {
        private bool isChecked = false;

        public bool IsChecked { get => isChecked; set => isChecked = value; }

        public CartItem() { 
            isChecked = false; 
        }
    }
}